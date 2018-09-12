using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Bcy.Models.BcyPost;

namespace ImageDL.Classes.ImageDownloading.Bcy
{
	/// <summary>
	/// Downloads images from Bcy.
	/// </summary>
	[DownloaderName("Bcy")]
	public sealed class BcyPostDownloader : PostDownloader
	{
		/// <summary>
		/// The id of the user to search for.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="BcyPostDownloader"/>
		/// </summary>
		public BcyPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Username, new[] { "user" })
			{
				Description = "The name or id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = await GetUserIdAsync(client, Username).CAF();
			var parsed = new List<Model>();
			//Iterate becasue there's a limit of 20 per page
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://bcy.net/home/timeline/loaduserposts" +
					$"?since={(parsed.Any() ? parsed.Last().Id : "0")}" +
					$"&uid={userId}" +
					$"&limit=20" +
					$"&source=all" +
					$"&filter=origin");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JObject.Parse(result.Value)["data"].Select(x => x["item_detail"].ToObject<Model>()).ToList();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					//First check indicates the post doesn't have any images
					//Due to the way this site's api is set up can't check image sizes
					if (post.PicNum < 1 || post.Score < MinScore)
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
		/// Gets the id of the Bcy user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<ulong> GetUserIdAsync(IDownloaderClient client, string username)
		{
			if (ulong.TryParse(username, out var val))
			{
				return val;
			}

			var query = new Uri($"https://bcy.net/search/user?k={username}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Bcy user id.");
			}

			try
			{
				var a = result.Value.DocumentNode.Descendants("a");
				var user = a.First(x => x.GetAttributeValue("title", "").CaseInsEquals(username));
				return Convert.ToUInt64(user.GetAttributeValue("href", null).Replace("/u/", ""));
			}
			catch (Exception e)
			{
				throw new HttpRequestException("Unable to get the Bcy user id.", e);
			}
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetBcyImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var result = await client.GetHtmlAsync(() => client.GenerateReq(url)).CAF();
			if (!result.IsSuccess)
			{
				return ImageResponse.FromNotFound(url);
			}
			var img = result.Value.DocumentNode.Descendants("img");
			var details = img.Where(x => x.GetAttributeValue("class", "").CaseInsContains("detail_std"));
			var src = details.Select(x => x.GetAttributeValue("src", ""));
			var urls = src.Select(x => new Uri(x.Substring(0, x.LastIndexOf('/'))));
			return src.Any() ? ImageResponse.FromImages(urls) : ImageResponse.FromNotFound(url);
		}
	}
}