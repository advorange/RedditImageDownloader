using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Attributes;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.TheAnimeGallery.Models.TheAnimeGalleryPost;

namespace ImageDL.Classes.ImageDownloading.TheAnimeGallery
{
	/// <summary>
	/// Downloads images from TheAnimeGallery.
	/// </summary>
	[DownloaderName("TheAnimeGallery")]
	public sealed class TheAnimeGalleryPostDownloader : PostDownloader
	{
		private static TAGContentFilter CurrentContentFilter = TAGContentFilter.Safe;

		/// <summary>
		/// The filter for content.
		/// </summary>
		public TAGContentFilter ContentFilter { get; set; }
		/// <summary>
		/// How to search for content.
		/// </summary>
		public TAGGatheringMethod GatheringMethod { get; set; }
		/// <summary>
		/// The thing to search for.
		/// </summary>
		public string Search { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="TheAnimeGalleryPostDownloader"/>.
		/// </summary>
		public TheAnimeGalleryPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Search), }, x => Search = x)
			{
				Description = "What to search for."
			});
			SettingParser.Add(new Setting<TAGContentFilter>(new[] { nameof(ContentFilter), "filter" }, x => ContentFilter = x, s => (Enum.TryParse(s, true, out TAGContentFilter result), result))
			{
				Description = "The filter to use when gathering posts. The default value is safe.",
				DefaultValue = TAGContentFilter.Safe,
			});
			SettingParser.Add(new Setting<TAGGatheringMethod>(new[] { nameof(GatheringMethod), "method" }, x => GatheringMethod = x, s => (Enum.TryParse(s, true, out TAGGatheringMethod result), result))
			{
				Description = "How to gather posts, either through a series or a tag.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			if (CurrentContentFilter != ContentFilter)
			{
				await SetContentFilter(client, ContentFilter).CAF();
			}

			var search = await GetSearchTerm(client, Search, GatheringMethod).CAF();
			var parsed = new List<Model>();
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 12); ++i)
			{
				var query = new Uri($"https://www.theanimegallery.com/gallery" +
					$"/{search}" +
					$"/category:all" +
					$"/by:date" +
					$"/page:{i + 1}");
				var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var div = result.Value.DocumentNode.Descendants("div");
				var thumbnails = div.Where(x => x.GetAttributeValue("class", null) == "thumbimage");
				var ids = thumbnails.Select(x => x.Descendants("a").Single().GetAttributeValue("rel", -1)).Where(x => x > -1).Distinct();
				var tasks = ids.GroupInto(4).Select(async x =>
				{
					var tmp = new List<Model>();
					foreach (var id in x)
					{
						tmp.Add(await GetTAGPostAsync(client, id.ToString()).CAF());
					}
					return tmp;
				});
				parsed = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x).ToList();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post, out _) || post.Score < MinScore)
					{
						continue;
					}
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
		/// <summary>
		/// Returns the url directing to the value.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="search"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		private static async Task<string> GetSearchTerm(IDownloaderClient client, string search, TAGGatheringMethod method)
		{
			//Tags can be simple numbers, so return them if they are
			if (method == TAGGatheringMethod.Tag && int.TryParse(search, out var num))
			{
				return num.ToString();
			}

			var query = new Uri($"https://www.theanimegallery.com/search/{search}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to find the correct search url with the supplied input.");
			}

			var div = result.Value.DocumentNode.Descendants("div");
			IEnumerable<HtmlNode> nodes;
			switch (method)
			{
				case TAGGatheringMethod.Tag:
					nodes = div.Where(x => x.GetAttributeValue("class", null) == "navGroup relative tagGroup");
					break;
				case TAGGatheringMethod.Series:
					nodes = div.Where(x => x.GetAttributeValue("class", null) == "navGroup relative");
					break;
				default:
					throw new ArgumentException("Invalid gathering method supplieds.");
			}

			try
			{
				var h1 = nodes.Select(x => x.Descendants("h1").ToList());
				var correct = h1.SingleOrDefault(x => x.Single(h => h.GetAttributeValue("class", null) == "right").InnerText == "100%");
				var val = correct[1].Descendants("a").First().GetAttributeValue("href", null).Split(':').Last();
				switch (method)
				{
					case TAGGatheringMethod.Tag:
						return $"tag:{val}";
					case TAGGatheringMethod.Series:
						return $"series:{val}";
					default:
						throw new ArgumentException("Invalid gathering method supplieds.");
				}
			}
			catch (Exception e)
			{
				throw new HttpRequestException("Unable to find the any term with a 100% match with the supplied input.", e);
			}
		}
		/// <summary>
		/// Sets the content filter to the supplied value.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static async Task SetContentFilter(IDownloaderClient client, TAGContentFilter filter)
		{
			//Not sure why this simply uses a user id instead of verifying someone is logged in.
			client.Cookies.Add(new Cookie("theanimegallery_id", "1", "/", "www.theanimegallery.com"));

			var data = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "rating", filter.ToString().ToLower() },
			});
			var query = new Uri("https://www.theanimegallery.com/ajax/updateContentFilterSetting.php");
			var result = await client.GetTextAsync(() =>
			{
				var req = client.GenerateReq(query, HttpMethod.Post);
				req.Content = data;
				req.Headers.Add("X-Requested-With", "XMLHttpRequest");
				return req;
			}).CAF();
			if (!result.IsSuccess)
			{
				throw new InvalidOperationException("Unable to set the content filter to the supplied value.");
			}
			CurrentContentFilter = filter;
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetTAGPostAsync(IDownloaderClient client, string id)
		{
			if (CurrentContentFilter != TAGContentFilter.Adult)
			{
				await SetContentFilter(client, TAGContentFilter.Adult).CAF();
			}

			var query = new Uri($"http://www.theanimegallery.com/gallery/image:{id}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? new Model(result.Value.DocumentNode) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetTAGImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/image:";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetTAGPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}