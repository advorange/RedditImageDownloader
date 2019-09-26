using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesSettingParser.Implementation.Instance;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.OAuth;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.OEmbed;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped;
using ImageDL.Enums;
using ImageDL.Interfaces;
using ImageDL.Utilities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Downloads images from DeviantArt.
	/// </summary>
	[DownloaderName("DeviantArt")]
	public sealed class DeviantArtPostDownloader : PostDownloader
	{
		private const string API = "https://www.deviantart.com/developers/";
		private const string SEARCH = "https://www.deviantartsupport.com/en/article/are-there-any-tricks-to-narrowing-down-a-search-on-deviantart";
		private static readonly Type _Type = typeof(DeviantArtPostDownloader);

		/// <summary>
		/// The client id to get the authorization token from.
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// The redirection website. Must be a valid website supplied in
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// The method to gather images with.
		/// </summary>
		public DeviantArtGatheringMethod GatheringMethod { get; set; }

		/// <summary>
		/// The tags to search for.
		/// </summary>
		public string Tags { get; set; }

		/// <summary>
		/// Creates an image downloader for DeviantArt.
		/// </summary>
		public DeviantArtPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => ClientId, new[] { "id" })
			{
				Description = $"The id of the client to get authentication from. For additional help, visit {API}.",
				IsOptional = true,
			});
			SettingParser.Add(new Setting<string>(() => ClientSecret, new[] { "secret" })
			{
				Description = $"The secret of the client to get authentication from. For additional help, visit {API}.",
				IsOptional = true,
			});
			SettingParser.Add(new Setting<string>(() => Tags)
			{
				Description = $"The tags to search for. For additional help, visit {SEARCH}.",
			});
			SettingParser.Add(new Setting<DeviantArtGatheringMethod>(() => GatheringMethod, new[] { "method" })
			{
				Description = $"How to gather posts. Api requies {nameof(ClientId)} and {nameof(ClientSecret)} to be set.",
			});
		}

		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetDeviantArtImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			if (await GetDeviantArtPostAsync(client, url).CAF() is DeviantArtOEmbedPost post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url">The url to gather. Can be in fav.me/id or post link format.</param>
		/// <returns></returns>
		public static async Task<DeviantArtOEmbedPost> GetDeviantArtPostAsync(IDownloaderClient client, Uri url)
		{
			var query = new Uri($"https://backend.deviantart.com/oembed?url={url}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (result.IsSuccess)
			{
				var jObj = JObject.Parse(result.Value);
				jObj.Add(nameof(DeviantArtOEmbedPost.PostUrl), url);
				return jObj.ToObject<DeviantArtOEmbedPost>();
			}
			return null;
		}

		/// <inheritdoc />
		protected override Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			return GatheringMethod switch
			{
				DeviantArtGatheringMethod.Scraping => GetPostsThroughScraping(client, list, token),
				DeviantArtGatheringMethod.Api => GetPostsThroughApi(client, list, token),
				DeviantArtGatheringMethod.Rss => GetPostsThroughRss(client, list, token),
				_ => throw new ArgumentOutOfRangeException(nameof(GatheringMethod)),
			};
		}

		/// <summary>
		/// Gets an api key for DeviantArt.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="clientId"></param>
		/// <param name="clientSecret"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKey(IDownloaderClient client, string clientId, string clientSecret)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}
			if (clientId != null && clientSecret != null)
			{
				var query = new Uri("https://www.deviantart.com/oauth2/token" +
					"?grant_type=client_credentials" +
					$"&client_id={clientId}" +
					$"&client_secret={clientSecret}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (result.IsSuccess)
				{
					var jObj = JObject.Parse(result.Value);
					var token = jObj["access_token"].ToObject<string>();
					var duration = TimeSpan.FromSeconds(jObj["expires_in"].ToObject<int>());
					return client.ApiKeys[_Type] = new ApiKey(token, duration);
				}
			}
			return default;
		}

		/// <summary>
		/// Generates the tags to be used in a query.
		/// </summary>
		/// <returns></returns>
		private string GenerateTags()
		{
			var query = WebUtility.UrlEncode(Tags);
			if (MaxDaysOld > 0)
			{
				query += $"+max_age:{MaxDaysOld}d";
			}
			return query.Trim('+');
		}

		/// <summary>
		/// Gets posts through the api.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private async Task GetPostsThroughApi(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new DeviantArtOAuthResults();
			//Iterate to get the new offset to start at
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.HasMore); i += parsed.Results?.Count ?? 0)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri("https://www.deviantart.com/api/v1/oauth2/browse/newest" +
					$"?offset={i}" +
					"&mature_content=true" +
					$"&q={GenerateTags()}" +
					$"&access_token={await GetApiKey(client, ClientId, ClientSecret).CAF()}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the access token, try to get another one
					if (result.Value.Contains("access_token"))
					{
						//Means the access token cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						i -= parsed.Results?.Count ?? 0;
						continue;
					}
					return;
				}

				parsed = JsonConvert.DeserializeObject<DeviantArtOAuthResults>(result.Value);
				foreach (var post in parsed.Results)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post.Content, out _) || post.Stats.Favorites < MinScore)
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
		/// Gets posts through the rss feed.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private async Task GetPostsThroughRss(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<DeviantArtRssPost>();
			//Iterate to get the new offset to start at
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); i += parsed.Count)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri("http://backend.deviantart.com/rss.xml" +
					$"?offset={i}" +
					$"&q={GenerateTags()}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var json = JsonUtils.ConvertXmlToJson(result.Value);
				parsed = JObject.Parse(json)["rss"]["channel"]["item"].ToObject<List<DeviantArtRssPost>>();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post.MediaContent, out _))
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
		/// Gets posts through scraping.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private async Task GetPostsThroughScraping(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<DeviantArtScrapedPost>();
			//Iterate to get the new offset to start at
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); i += parsed.Count)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri("https://www.deviantart.com/newest/" +
					$"?offset={i}" +
					$"&q={GenerateTags()}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				const string jsonSearch = "window.__pageload =";
				var jsonCut = result.Value.Substring(result.Value.IndexOf(jsonSearch) + jsonSearch.Length);

				//Now we have all the json, but we only want the artwork json so we have to parse that manually
				parsed = JObject.Parse(jsonCut.Substring(0, jsonCut.IndexOf("}}}</script>") + 3).Trim())["metadata"].Select(x =>
				{
					try
					{
						return x.First.ToObject<DeviantArtScrapedPost>();
					}
					catch (JsonSerializationException) //Ignore any serialization exceptions, just don't include them
					{
						return null;
					}
				}).Where(x => x != null).ToList();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (!HasValidSize(post, out _)) //Can't check score or time when scraping
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
	}
}