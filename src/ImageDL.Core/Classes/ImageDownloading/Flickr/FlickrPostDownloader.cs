using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Flickr.Models.FlickrPost;

namespace ImageDL.Classes.ImageDownloading.Flickr
{
	/// <summary>
	/// Downloads image from Flickr.
	/// </summary>
	[DownloaderName("Flickr")]
	public sealed class FlickrPostDownloader : PostDownloader
	{
		private static readonly Type _Type = typeof(FlickrPostDownloader);

		/// <summary>
		/// The tags to search for posts with.
		/// </summary>
		public string Search { get; set; }
		/// <summary>
		/// The method to gather images with.
		/// </summary>
		public FlickrGatheringMethod GatheringMethod { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="FlickrPostDownloader"/>.
		/// </summary>
		public FlickrPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Search), }, x => Search = x)
			{
				Description = $"What to search for, can be a username or tags.",
			});
			SettingParser.Add(new Setting<FlickrGatheringMethod>(new[] { nameof(GatheringMethod), "method" }, x => GatheringMethod = x,
				parser: s => (Enum.TryParse(s, true, out FlickrGatheringMethod result), result))
			{
				Description = $"How to use {Search} for searching.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = "";
			var parsed = new List<Model>();
			//Iterate to get the next page of results
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 100); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = await GenerateApiQueryAsync(client, i).CAF();
				switch (GatheringMethod)
				{
					case FlickrGatheringMethod.Tags:
						query += $"&method=flickr.photos.search";
						query += $"&text={Search}";
						break;
					case FlickrGatheringMethod.User:
						if (String.IsNullOrWhiteSpace(userId))
						{
							userId = await GetUserIdAsync(client, Search).CAF();
						}
						query += $"&method=flickr.people.getPhotos";
						query += $"&user_id={userId}";
						break;
				}
				var result = await client.GetTextAsync(() => client.GenerateReq(new Uri(query))).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the api key, try to get another one
					if (result.Value.Contains("API Key"))
					{
						//Means the api key cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						--i; //Decrement since this iteration is useless
						continue;
					}
				}

				parsed = JObject.Parse(result.Value)["photos"]["photo"].ToObject<List<Model>>();
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
		/// Generates a query for the api.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static async Task<string> GenerateApiQueryAsync(IDownloaderClient client, int page)
		{
			return $"https://api.flickr.com/services/rest" +
				$"?sort=date-posted-desc" +
				$"&parse_tags=1" +
				$"&content_type=7" +
				$"&extras=date_upload,count_comments,count_faves,media,owner_name,url_m,url_o" +
				$"&per_page=100" +
				$"&page={page + 1}" +
				$"&lang=en-US" +
				$"&api_key={await GetApiKeyAsync(client).CAF()}" +
				$"&format=json" +
				$"&nojsoncallback=1";
		}
		/// <summary>
		/// Gets an api key for Flickr.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKeyAsync(IDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}

			var query = new Uri("https://www.flickr.com");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Flickr api key.");
			}

			var search = "api.site_key = \"";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return (client.ApiKeys[_Type] = new ApiKey(cut.Substring(0, cut.IndexOf('"'))));
		}
		/// <summary>
		/// Gets the id of the user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<string> GetUserIdAsync(IDownloaderClient client, string username)
		{
			var query = new Uri($"https://www.flickr.com/photos/{username}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Flickr user's id.");
			}

			var search = "\"ownerNsid\":\"";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return cut.Substring(0, cut.IndexOf('"'));
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetFlickrPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"{await GenerateApiQueryAsync(client, 0).CAF()}&method=flickr.photos.getInfo&photo_id={id}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}
			return JObject.Parse(result.Value)["photo"].ToObject<Model>(JsonSerializer.Create(new JsonSerializerSettings
			{
				Error = (obj, e) =>
				{
					//Eat any exceptions for unexpected start objects, since json doesn't like unmapped nested objects
					e.ErrorContext.Handled = e.ErrorContext.Error.ToString().Contains("StartObject. Path 'photo.");
				},
			}));
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetFlickrImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/photos/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
				if (await GetFlickrPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}