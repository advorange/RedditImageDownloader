using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Pixiv.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Interfaces.IPost;

namespace ImageDL.Classes.ImageDownloading.Pixiv
{
	/// <summary>
	/// Downloads images from Pixiv.
	/// </summary>
	[DownloaderName("Pixiv")]
	public sealed class PixivImageDownloader : ImageDownloader
	{
		//No clue how the client id and secret are gotten, but these are still valid from a github repo last updated in 2016
		private static readonly string _ClientId = "bYGKuGVw91e0NMfPGp44euvGt59s";
		private static readonly string _ClientSecret = "HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";
		private static readonly Type _Type = typeof(PixivImageDownloader);

		/// <summary>
		/// The username to login with.
		/// </summary>
		public string LoginUsername
		{
			get => _LoginUsername;
			set => _LoginUsername = value;
		}
		/// <summary>
		/// The password to login with.
		/// </summary>
		public string LoginPassword
		{
			get => _LoginPassword;
			set => _LoginPassword = value;
		}
		/// <summary>
		/// The id of the user to search for.
		/// </summary>
		public ulong UserId
		{
			get => _UserId;
			set => _UserId = value;
		}

		private string _LoginUsername;
		private string _LoginPassword;
		private ulong _UserId;

		/// <summary>
		/// Creates an instance of <see cref="PixivImageDownloader"/>.
		/// </summary>
		public PixivImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(LoginUsername), "username" }, x => LoginUsername = x)
			{
				Description = "The username to login with. This is what you use to login to Pixiv regularly.",
			});
			SettingParser.Add(new Setting<string>(new[] { nameof(LoginPassword), "password" }, x => LoginPassword = x)
			{
				Description = "The password to login with. This is what you use to login to Pixiv regularly.",
			});
			SettingParser.Add(new Setting<ulong>(new[] { nameof(UserId), "id" }, x => UserId = x)
			{
				Description = "The id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var parsed = new PixivPage();
			//Iterate because it's easy and has less strain
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 100); ++i)
			{
				var query = new Uri($"https://public-api.secure.pixiv.net/v1/users/{UserId}/works.json" +
					$"?access_token={await GetApiKeyAsync(client, LoginUsername, LoginPassword).CAF()}" +
					$"&page={i + 1}" +
					$"&per_page=100" +
					$"&include_stats=1" +
					$"&include_sanity_level=1" +
					$"&image_sizes=large"); //This is an array of sizes separated by commas, but we only care about large
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the access token, try to get another one
					if (result.Value.Contains("access token"))
					{
						//Means the access token cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						--i; //Decrement since this iteration is useless
						continue;
					}
					return;
				}

				parsed = JsonConvert.DeserializeObject<PixivPage>(result.Value);
				foreach (var post in parsed.Posts)
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Score < MinScore)
					{
						continue;
					}
					//Don't think the API has an endpoint that holds the sizes of every image?
					if (!HasValidSize(post, out _))
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
		/// Logs into pixiv, generating an access token.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client, string username, string password)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key) && (key.CreatedAt + key.ValidFor) > DateTime.UtcNow)
			{
				return key;
			}

			var data = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "get_secure_url", "1" },
				{ "client_id", _ClientId },
				{ "client_secret", _ClientSecret },
				{ "grant_type", "password" },
				{ "username", username },
				{ "password", password },
			});

			var query = new Uri("https://oauth.secure.pixiv.net/auth/token");
			var req = client.GenerateReq(query, HttpMethod.Post);
			req.Content = data;

			using (var resp = await client.SendAsync(req).CAF())
			{
				if (!resp.IsSuccessStatusCode)
				{
					throw new InvalidOperationException("Unable to login to Pixiv.");
				}

				var jObj = JObject.Parse(await resp.Content.ReadAsStringAsync().CAF());
				var accessToken = jObj["response"]["access_token"].ToObject<string>();
				var expiresIn = TimeSpan.FromSeconds(jObj["response"]["expires_in"].ToObject<int>());
				return (client.ApiKeys[_Type] = new ApiKey(accessToken, expiresIn));
			}
		}
	}
}