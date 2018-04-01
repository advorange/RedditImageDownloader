using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Imgur.ImgurPost;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Downloads images from Imgur.
	/// </summary>
	public sealed class ImgurImageDownloader : ImageDownloader<Model>
	{
		private const string TAGS = "https://apidocs.imgur.com/#3c981acf-47aa-488f-b068-269f65aee3ce";

		/// <summary>
		/// The tags to search for posts with.
		/// </summary>
		public string Tags
		{
			get => _Tags;
			set => _Tags = value;
		}

		private string _Tags;

		/// <summary>
		/// Creates an instance of <see cref="ImgurImageDownloader"/>.
		/// </summary>
		public ImgurImageDownloader() : base("Imgur")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags) }, x => Tags = x)
			{
				Description = $"The tags to search for. For help see {TAGS}.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<Model> list)
		{
			var parsed = new List<Model>();
			var keepGoing = true;
			//Iterate to get the next page of results
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 60); ++i)
			{
				//If the key is empty either this just started or it was reset due to the key becoming invalid
				if (String.IsNullOrWhiteSpace(Client[Name]))
				{
					var clientId = await GetClientId().CAF();
					if (clientId == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.ApiKeys[Name] = new ApiKey(clientId);
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateGalleryQuery(Client[Name], i)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the api key, try to get another one
					if (result.Text.Contains("client_id"))
					{
						Client.ApiKeys[Name] = new ApiKey(null);
						--i;
						continue;
					}
					break;
				}

				parsed = JObject.Parse(result.Text)["data"].ToObject<List<Model>>();
				foreach (var post in parsed)
				{
					if (!(keepGoing = post.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (post.Score < MinScore)
					{
						continue;
					}
					else if (post.IsAlbum)
					{
						//Make sure we have all the images
						await GatherAllImagesAsync(Client[Name], post).CAF();
					}
					//Remove all images that don't meet the size requirements
					for (int j = post.ImagesCount - 1; j >= 0; --j)
					{
						var image = post.Images[j];
						if (!FitsSizeRequirements(null, image.Width, image.Height, out _))
						{
							post.Images.RemoveAt(j);
						}
					}
					if (!post.Images.Any())
					{
						continue;
					}
					else if (!(keepGoing = Add(list, post)))
					{
						break;
					}
				}
			}
		}
		/// <inheritdoc />
		protected override List<Model> OrderAndRemoveDuplicates(List<Model> list)
		{
			return list.OrderByDescending(x => x.Score).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(Model post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.FavoriteCount}] {post.ImageLink}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(Model post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override Task<ScrapeResult> GatherImagesAsync(Model post)
		{
			var images = post.Images.Select(x => new Uri(x.ImageLink));
			return Task.FromResult(new ScrapeResult(new Uri(post.Link), false, new ImgurScraper(), images, null));
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(Model post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.FavoriteCount ?? 0, reason);
		}

		private Uri GenerateGalleryQuery(string clientId, int page)
		{
			return new Uri($"https://api.imgur.com/3/gallery/search/time/all/{page}/" +
				$"?client_id={clientId}" +
				$"&q={WebUtility.UrlEncode(Tags)}");
		}
		private async Task<string> GetClientId()
		{
			//Load the page regularly first so we can get some data from it
			var fLink = $"https://imgur.com/t/{WebUtility.UrlEncode(Tags)}";
			var fResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(fLink)).CAF();
			if (!fResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the tags page.");
			}

			//Put it in a doc so we can actually parse it
			var doc = new HtmlDocument();
			doc.LoadHtml(fResult.Text);

			//Find the direct link to main.gibberish.js
			var jsLink = doc.DocumentNode.Descendants("script")
				.Select(x => x.GetAttributeValue("src", null))
				.First(x => (x ?? "").Contains("/main."));
			var jsResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(jsLink)).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the client id.");
			}

			//Read main.gibberish.js and find the client id
			var idSearch = "apiClientId:\"";
			var idCut = jsResult.Text.Substring(jsResult.Text.IndexOf(idSearch) + idSearch.Length);
			return idCut.Substring(0, idCut.IndexOf('"'));
		}
		private async Task GatherAllImagesAsync(string clientId, Model post)
		{
			//Only bother checking if there is a difference between the stated count and the gathered count
			if (!(post.ImagesCount != post.Images?.Count))
			{
				return;
			}

			var query = $"https://api.imgur.com/3/album/{post.Id}/images?client_id={clientId}";
			var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException($"Unable to get all the images for {post.Id}.");
			}

			foreach (var image in JObject.Parse(result.Text)["data"].ToObject<List<ImgurImage>>())
			{
				if (post.Images.Select(x => x.Id).Contains(image.Id))
				{
					continue;
				}
				post.Images.Add(image);
			}
		}
	}
}