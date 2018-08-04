using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Imgur.Models;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Imgur.Models.ImgurPost;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Downloads images from Imgur.
	/// </summary>
	[DownloaderName("Imgur")]
	public sealed class ImgurPostDownloader : PostDownloader
	{
		private static readonly Type _Type = typeof(ImgurPostDownloader);

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
		/// Creates an instance of <see cref="ImgurPostDownloader"/>.
		/// </summary>
		public ImgurPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags) }, x => Tags = x)
			{
				Description = $"The tags to search for. For help see https://apidocs.imgur.com/#3c981acf-47aa-488f-b068-269f65aee3ce.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<Model>();
			//Iterate to get the next page of results
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 60); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://api.imgur.com/3/gallery/search/time/all/{i}/" +
					$"?client_id={await GetApiKeyAsync(client).CAF()}" +
					$"&q={WebUtility.UrlEncode(Tags)}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the api key, try to get another one
					if (result.Value.Contains("client_id"))
					{
						//Means the access token cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						--i;
						continue;
					}
					return;
				}

				parsed = JObject.Parse(result.Value)["data"].ToObject<List<Model>>();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Score < MinScore)
					{
						continue;
					}
					//Get all images then make sure they're valid
					await post.SetAllImages(client).CAF();
					foreach (var image in post.Images.Where(x => !HasValidSize(x, out _)).ToList())
					{
						post.Images.Remove(image);
					}
					if (!post.Images.Any())
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
		/// Gets an api key for imgur.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKeyAsync(IDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}
			//Load the page regularly first so we can get some data from it
			var query = new Uri($"https://imgur.com/t/dogs");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the tags page.");
			}
			//Find the direct link to main.gibberish.js
			var jsQuery = new Uri(result.Value.DocumentNode.Descendants("script")
				.Select(x => x.GetAttributeValue("src", ""))
				.First(x => x.Contains("/main.")));
			var jsResult = await client.GetTextAsync(() => client.GenerateReq(jsQuery)).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the client id.");
			}
			//Read main.gibberish.js and find the client id
			var idSearch = "apiClientId:\"";
			var idCut = jsResult.Value.Substring(jsResult.Value.IndexOf(idSearch) + idSearch.Length);
			return (client.ApiKeys[_Type] = new ApiKey(idCut.Substring(0, idCut.IndexOf('"'))));
		}
		/// <summary>
		/// Gets the images from the supplied album id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static async Task<List<ImgurImage>> GetImgurImagesByCode(IDownloaderClient client, string code)
		{
			//Albums are more commonly the 5 digit length code
			var isAlbum = code.Length == 5;
			for (int notFoundCount = 0;;)
			{
				var endpoint = isAlbum ? "album" : "image";
				var query = new Uri($"https://api.imgur.com/3/{endpoint}/{code}?client_id={await GetApiKeyAsync(client).CAF()}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (result.IsSuccess)
				{
					var data = JObject.Parse(result.Value)["data"];
					return isAlbum
						? data["images"].ToObject<List<ImgurImage>>()
						: new List<ImgurImage> { data.ToObject<ImgurImage>() };
				}
				//Chance we may hit a 404 page since we don't know for certain what type of object the code leads to
				if (result.StatusCode == HttpStatusCode.NotFound)
				{
					//Lead to a 404 page twice, meaning it doesn't exist as either an album or image
					if (++notFoundCount > 1)
					{
						return new List<ImgurImage>();
					}
					//Attempt to find the object as an image if we tried album before, or vice versa
					isAlbum = !isAlbum;
					continue;
				}
				//If there's an error with the api key, try to get another one
				if (result.Value.Contains("client_id"))
				{
					client.ApiKeys.Remove(_Type);
					continue;
				}
				return new List<ImgurImage>();
			}
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetImgurImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString().Replace("_d", "");
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var id = u.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
			var images = await GetImgurImagesByCode(client, id).CAF();
			if (images.Any())
			{
				var tasks = images.Select(async x => await x.GetImagesAsync(client).CAF());
				var urls = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x.ImageUrls).ToArray();
				return ImageResponse.FromImages(urls);
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}