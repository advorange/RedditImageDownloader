using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Imgur.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Imgur.Models.ImgurPost;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Downloads images from Imgur.
	/// </summary>
	public sealed class ImgurImageDownloader : ImageDownloader<Model>
	{
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
				Description = $"The tags to search for. For help see https://apidocs.imgur.com/#3c981acf-47aa-488f-b068-269f65aee3ce.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			var parsed = new List<Model>();
			var keepGoing = true;
			//Iterate to get the next page of results
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 60); ++i)
			{
				var query = $"https://api.imgur.com/3/gallery/search/time/all/{i}/" +
					$"?client_id={await GetApiKeyAsync(client).CAF()}" +
					$"&q={WebUtility.UrlEncode(Tags)}";
				var result = await client.GetText(new Uri(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the api key, try to get another one
					if (result.Value.Contains("client_id"))
					{
						client.ApiKeys.Remove(typeof(ImgurImageDownloader));
						--i;
						continue;
					}
					break;
				}

				parsed = JObject.Parse(result.Value)["data"].ToObject<List<Model>>();
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
					//Make sure we have all the images.
					await post.SetAllImages(client).CAF();
					//Remove all images that don't meet the size requirements
					post.Images.RemoveAll(x => !HasValidSize(null, x.Width, x.Height, out _));
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

		/// <summary>
		/// Gets an api key for imgur.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(typeof(ImgurImageDownloader), out var key))
			{
				return key;
			}
			//Load the page regularly first so we can get some data from it
			var query = $"https://imgur.com/t/dogs";
			var result = await client.GetHtml(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the tags page.");
			}
			//Find the direct link to main.gibberish.js
			var jsQuery = result.Value.DocumentNode.Descendants("script")
				.Select(x => x.GetAttributeValue("src", null))
				.First(x => (x ?? "").Contains("/main."));
			var jsResult = await client.GetText(new Uri(jsQuery)).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the client id.");
			}
			//Read main.gibberish.js and find the client id
			var idSearch = "apiClientId:\"";
			var idCut = jsResult.Value.Substring(jsResult.Value.IndexOf(idSearch) + idSearch.Length);
			return (client.ApiKeys[typeof(ImgurImageDownloader)] = new ApiKey(idCut.Substring(0, idCut.IndexOf('"'))));
		}
		/// <summary>
		/// Gets the images from the supplied album id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static async Task<List<ImgurImage>> GetImagesAsync(IImageDownloaderClient client, string code)
		{
			while (true)
			{
				var query = new Uri($"https://api.imgur.com/3/album/{code}/images?client_id={await GetApiKeyAsync(client).CAF()}");
				var result = await client.GetText(query).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the api key, try to get another one
					if (result.Value.Contains("client_id"))
					{
						client.ApiKeys.Remove(typeof(ImgurImageDownloader));
						continue;
					}
					return new List<ImgurImage>();
				}
				return JObject.Parse(result.Value)["data"].ToObject<List<ImgurImage>>();
			}
		}
	}
}