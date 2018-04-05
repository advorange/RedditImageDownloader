using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.DeviantArt.Models.DeviantArtPost;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Downloads images from DeviantArt.
	/// </summary>
	public sealed class DeviantArtImageDownloader : ImageDownloader<Model>
	{
		private const string SEARCH = "https://www.deviantartsupport.com/en/article/are-there-any-tricks-to-narrowing-down-a-search-on-deviantart";
		private const string API = "https://www.deviantart.com/developers/";

		/// <summary>
		/// The client id to get the authorization token from.
		/// </summary>
		public string ClientId
		{
			get => _ClientId;
			set => _ClientId = value;
		}
		/// <summary>
		/// The redirection website. Must be a valid website supplied in 
		/// </summary>
		public string ClientSecret
		{
			get => _ClientSecret;
			set => _ClientSecret = value;
		}
		/// <summary>
		/// The tags to search for.
		/// </summary>
		public string TagString
		{
			get => _TagString;
			set => _TagString = value;
		}

		private string _ClientId;
		private string _ClientSecret;
		private string _TagString;

		/// <summary>
		/// Creates an image downloader for DeviantArt.
		/// </summary>
		public DeviantArtImageDownloader() : base("DeviantArt")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(ClientId), "id" }, x => ClientId = x)
			{
				Description = $"The id of the client to get authentication from. For additional help, visit {API}.",
				IsOptional = true,
			});
			SettingParser.Add(new Setting<string>(new[] { nameof(ClientSecret), "secret" }, x => ClientSecret = x)
			{
				Description = $"The secret of the client to get authentication from. For additional help, visit {API}.",
				IsOptional = true,
			});
			SettingParser.Add(new Setting<string>(new[] { nameof(TagString), "tags" }, x => TagString = x)
			{
				Description = $"the tags to search for. For additional help, visit {SEARCH}.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> validPosts)
		{
			try
			{
				if ((await GetApiKey(client, ClientId, ClientSecret).CAF()).Key != null)
				{
					await GetPostsThroughApi(client, validPosts).CAF();
				}
				else
				{
					await GetPostsThroughScraping(client, validPosts).CAF();
				}
			}
			catch (WebException we) when (we.Message.Contains("403")) { } //Gotten when scraping since don't know when to stop.
		}

		/// <summary>
		/// Gets an api key for DeviantArt.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="clientId"></param>
		/// <param name="clientSecret"></param>
		/// <returns></returns>
		public static async Task<ApiKey> GetApiKey(IImageDownloaderClient client, string clientId, string clientSecret)
		{
			if (client.ApiKeys.TryGetValue(typeof(DeviantArtImageDownloader), out var key))
			{
				return key;
			}
			if (clientId != null && clientSecret != null)
			{
				var query = $"https://www.deviantart.com/oauth2/token" +
					$"?grant_type=client_credentials" +
					$"&client_id={clientId}" +
					$"&client_secret={clientSecret}";
				var result = await client.GetText(new Uri(query)).CAF();
				if (result.IsSuccess)
				{
					var jObj = JObject.Parse(result.Value);
					var token = jObj["access_token"].ToObject<string>();
					var duration = TimeSpan.FromSeconds(jObj["expires_in"].ToObject<int>());
					return (client.ApiKeys[typeof(DeviantArtImageDownloader)] = new ApiKey(token, duration));
				}
			}
			return default;
		}
		private string GenerateTags()
		{
			var query = WebUtility.UrlEncode(TagString);
			if (MaxDaysOld > 0)
			{
				query += $"+max_age:{MaxDaysOld}d";
			}
			return query.Trim('+');
		}
		private async Task GetPostsThroughScraping(IImageDownloaderClient client, List<Model> list)
		{
			var parsed = new List<DeviantArtScrapedPost>();
			var keepGoing = true;
			//Iterate to get the new offset to start at
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); i += parsed.Count)
			{
				var query = $"https://www.deviantart.com/newest/" +
					$"?offset={i}" +
					$"&q={GenerateTags()}";
				var result = await client.GetText(new Uri(query)).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				var jsonSearch = "window.__pageload =";
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
					if (!HasValidSize(null, post.Width, post.Height, out _)) //Can't check score or time when scraping
					{
						continue;
					}
					else if (!(keepGoing = Add(list, new Model(post))))
					{
						break;
					}
				}
			}
		}
		private async Task GetPostsThroughApi(IImageDownloaderClient client, List<Model> list)
		{
			var parsed = new DeviantArtApiResults();
			var keepGoing = true;
			//Iterate to get the new offset to start at
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.HasMore); i += parsed.Results.Count)
			{
				var query = $"https://www.deviantart.com/api/v1/oauth2/browse/newest" +
					$"?offset={i}" +
					$"&mature_content=true" +
					$"&q={GenerateTags()}" +
					$"&access_token={await GetApiKey(client, ClientId, ClientSecret)}";
				var result = await client.GetText(new Uri(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the access token, try to get another one
					if (result.Value.Contains("access_token"))
					{
						client.ApiKeys.Remove(typeof(DeviantArtImageDownloader));
						i -= parsed.Results.Count;
						continue;
					}
					break;
				}

				parsed = JsonConvert.DeserializeObject<DeviantArtApiResults>(result.Value);
				foreach (var post in parsed.Results)
				{
					if (!(keepGoing = post.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (post.Content.Source == null //Is a journal or something like that
						|| !HasValidSize(null, post.Content.Width, post.Content.Height, out _)
						|| post.Stats.Favorites < MinScore)
					{
						continue;
					}
					else if (!(keepGoing = Add(list, new Model(post))))
					{
						break;
					}
				}
			}
		}
	}
}