using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Downloads images from DeviantArt.
	/// </summary>
	public sealed class DeviantArtImageDownloader : ImageDownloader<DeviantArtPost>
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
		public DeviantArtImageDownloader()
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
		protected override async Task GatherPostsAsync(List<DeviantArtPost> validPosts)
		{
			try
			{
				var (token, duration) = await GetTokenAsync().CAF();
				if (token != null)
				{
					Client.UpdateAPIKey(token, duration);
					await GetPostsThroughApi(validPosts).CAF();
				}
				else
				{
					await GetPostsThroughScraping(validPosts).CAF();
				}
			}
			catch (WebException we) when (we.Message.Contains("403")) { } //Gotten when scraping since don't know when to stop.
		}
		/// <inheritdoc />
		protected override List<DeviantArtPost> OrderAndRemoveDuplicates(List<DeviantArtPost> list)
		{
			return list.GroupBy(x => x.Source).Select(x => x.First()).OrderByDescending(x => x.Favorites).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(DeviantArtPost post, int count)
		{
			var postHasScore = post.Favorites > 0 ? $"|\u2191{post.Favorites}" : "";
			Console.WriteLine($"[#{count}{postHasScore}] {post.Source}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(DeviantArtPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.PostId}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(DeviantArtPost post)
		{
			return await Client.ScrapeImagesAsync(new Uri(post.Source)).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DeviantArtPost post, Uri uri, string reason)
		{
			//If favorites are there then use that, otherwise just use the post id
			return new ContentLink(uri, post.Favorites < 0 ? post.PostId : post.Favorites, reason);
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
		private string GenerateScrapingQuery(int offset)
		{
			return $"https://www.deviantart.com/newest/" +
				$"?offset={offset}" +
				$"&q={GenerateTags()}";
		}
		private string GenerateApiQuery(int offset)
		{
			return $"https://www.deviantart.com/api/v1/oauth2/browse/newest" +
				$"?offset={offset}" +
				$"&mature_content=true" +
				$"&q={GenerateTags()}" +
				$"&access_token={Client.APIKey}";
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
		private async Task GetPostsThroughScraping(List<DeviantArtPost> validPosts)
		{
			for (int i = 0; validPosts.Count < AmountToDownload;)
			{
				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(GenerateScrapingQuery(i))).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				var jsonStart = "window.__pageload =";
				var jsonStartIndex = result.Text.IndexOf(jsonStart) + jsonStart.Length;
				var jsonEnd = "}}}</script>";
				var jsonEndIndex = result.Text.IndexOf(jsonEnd) + 3;
				var json = result.Text.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex).Trim();

				//Now we have all the json, but we only want the artwork json so we have to parse that manually
				var parsed = JObject.Parse(json)["metadata"].Select(x =>
				{
					try
					{
						return x.First.ToObject<ScrapedDeviantArtPost>();
					}
					catch (JsonSerializationException) //Ignore any serialization exceptions, just don't include them
					{
						return null;
					}
				}).Where(x => x != null).ToList();
				var finished = false;
				foreach (var post in parsed)
				{
					if (!FitsSizeRequirements(null, post.Width, post.Height, out _)) //Can't check score/favorites when scraping
					{
						continue;
					}

					validPosts.Add(new DeviantArtPost(post));
					if (validPosts.Count == AmountToDownload)
					{
						finished = true;
						break;
					}
					else if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} DeviantArt posts found.");
					}
				}

				//24 is a full page, but for some reason only 20 or so can be gotten usually
				if (finished || parsed.Count < 20)
				{
					break;
				}
				i += parsed.Count;
			}
		}
		private async Task GetPostsThroughApi(List<DeviantArtPost> validPosts)
		{
			for (int i = 0; validPosts.Count < AmountToDownload;)
			{
				if (Client.APIKeyLastUpdated + Client.APIKeyDuration >= DateTime.UtcNow)
				{
					var (newToken, duration) = await GetTokenAsync().CAF();
					if (newToken == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.UpdateAPIKey(newToken, duration);
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(GenerateApiQuery(i))).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				var parsed = JsonConvert.DeserializeObject<ApiDeviantArtResults>(result.Text);
				var finished = false;
				foreach (var post in parsed.Results)
				{
					if (post.Content.Source == null) //Is a journal or something like that
					{
						continue;
					}
					else if (!FitsSizeRequirements(null, post.Content.Width, post.Content.Height, out _) || post.Stats.Favorites < MinScore)
					{
						continue;
					}

					validPosts.Add(new DeviantArtPost(post));
					if (validPosts.Count == AmountToDownload)
					{
						finished = true;
						break;
					}
					else if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} DeviantArt posts found.");
					}
				}

				//Break out if finished or no more are left
				if (finished || !parsed.HasMore)
				{
					break;
				}
				i += parsed.Results.Count;
			}
		}
	}
}