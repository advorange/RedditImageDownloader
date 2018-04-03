using AdvorangesUtils;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.DeviantArt.DeviantArtPost;

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
		/// <param name="client">The client to download images with.</param>
		public DeviantArtImageDownloader(ImageDownloaderClient client) : base(client, new Uri("https://www.deviantart.com"))
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
		protected override async Task GatherPostsAsync(List<Model> validPosts)
		{
			try
			{
				var (Token, Duration) = await GetTokenAsync().CAF();
				if (Token != null)
				{
					Client.ApiKeys[Name] = new ApiKey(Token, Duration);
					await GetPostsThroughApi(validPosts).CAF();
				}
				else
				{
					await GetPostsThroughScraping(validPosts).CAF();
				}
			}
			catch (WebException we) when (we.Message.Contains("403")) { } //Gotten when scraping since don't know when to stop.
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
		private Uri GenerateScrapingQuery(int offset)
		{
			return new Uri($"https://www.deviantart.com/newest/" +
				$"?offset={offset}" +
				$"&q={GenerateTags()}");
		}
		private Uri GenerateApiQuery(string token, int offset)
		{
			return new Uri($"https://www.deviantart.com/api/v1/oauth2/browse/newest" +
				$"?offset={offset}" +
				$"&mature_content=true" +
				$"&q={GenerateTags()}" +
				$"&access_token={token}");
		}
		private async Task<(string Token, TimeSpan Duration)> GetTokenAsync()
		{
			string token = null;
			TimeSpan duration = default;
			if (ClientId != null && ClientSecret != null)
			{
				var request = $"https://www.deviantart.com/oauth2/token" +
					$"?grant_type=client_credentials" +
					$"&client_id={ClientId}" +
					$"&client_secret={ClientSecret}";

				using (var resp = await Client.SendWithRefererAsync(new Uri(request), HttpMethod.Get).CAF())
				{
					if (resp.IsSuccessStatusCode)
					{
						var jObj = JObject.Parse(await resp.Content.ReadAsStringAsync().CAF());
						token = jObj["access_token"].ToObject<string>();
						duration = TimeSpan.FromSeconds(jObj["expires_in"].ToObject<int>());
					}
				}
			}
			return (token, duration);
		}
		private async Task GetPostsThroughScraping(List<Model> list)
		{
			var parsed = new List<DeviantArtScrappedPost>();
			var keepGoing = true;
			//Iterate to get the new offset to start at
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); i += parsed.Count)
			{
				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateScrapingQuery(i)).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				var jsonSearch = "window.__pageload =";
				var jsonCut = result.Text.Substring(result.Text.IndexOf(jsonSearch) + jsonSearch.Length);

				//Now we have all the json, but we only want the artwork json so we have to parse that manually
				parsed = JObject.Parse(jsonCut.Substring(0, jsonCut.IndexOf("}}}</script>") + 3).Trim())["metadata"].Select(x =>
				{
					try
					{
						return x.First.ToObject<DeviantArtScrappedPost>();
					}
					catch (JsonSerializationException) //Ignore any serialization exceptions, just don't include them
					{
						return null;
					}
				}).Where(x => x != null).ToList();
				foreach (var post in parsed)
				{
					if (!FitsSizeRequirements(null, post.Width, post.Height, out _)) //Can't check score or time when scraping
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
		private async Task GetPostsThroughApi(List<Model> list)
		{
			var parsed = new DeviantArtApiResults();
			var keepGoing = true;
			//Iterate to get the new offset to start at
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.HasMore); i += parsed.Results.Count)
			{
				if (String.IsNullOrWhiteSpace(Client[Name]))
				{
					var (Token, Duration) = await GetTokenAsync().CAF();
					if (Token == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.ApiKeys[Name] = new ApiKey(Token, Duration);
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateApiQuery(Client[Name], i)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the access token, try to get another one
					if (result.Text.Contains("access_token"))
					{
						Client.ApiKeys[Name] = new ApiKey(null);
						i -= parsed.Results.Count;
						continue;
					}
					break;
				}

				parsed = JsonConvert.DeserializeObject<DeviantArtApiResults>(result.Text);
				foreach (var post in parsed.Results)
				{
					if (!(keepGoing = post.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (post.Content.Source == null //Is a journal or something like that
						|| !FitsSizeRequirements(null, post.Content.Width, post.Content.Height, out _)
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