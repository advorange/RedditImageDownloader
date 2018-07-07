using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Pixiv.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Pixiv
{
	/// <summary>
	/// Downloads images from Pixiv.
	/// </summary>
	[DownloaderName("Pixiv")]
	public sealed class PixivPostDownloader : PostDownloader
	{
		//No clue how the client id and secret are gotten, but these are still valid from a github repo last updated in 2016
		private static readonly string _ClientId = "bYGKuGVw91e0NMfPGp44euvGt59s";
		private static readonly string _ClientSecret = "HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";
		private static readonly Type _Type = typeof(PixivPostDownloader);
		//Find _p0_ in a url so we can remove the stuff after it like _master1200
		private static readonly Regex _FindP = new Regex(@"_p(\d*?)_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		//Remove size arguments from an image (i.e. /c/600x600/ is saying to have a 600x600 image which we don't want)
		private static readonly Regex _RemoveC = new Regex(@"\/c\/(\d*?)x(\d*?)\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
		/// Creates an instance of <see cref="PixivPostDownloader"/>.
		/// </summary>
		public PixivPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(LoginUsername), }, x => LoginUsername = x)
			{
				Description = "The username to login with. This is what you use to login to Pixiv regularly.",
			});
			SettingParser.Add(new Setting<string>(new[] { nameof(LoginPassword), }, x => LoginPassword = x)
			{
				Description = "The password to login with. This is what you use to login to Pixiv regularly.",
			});
			SettingParser.Add(new Setting<ulong>(new[] { nameof(UserId), "id" }, x => UserId = x)
			{
				Description = "The id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new PixivPage();
			//Iterate because it's easy and has less strain
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 100); ++i)
			{
				token.ThrowIfCancellationRequested();
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
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Score < MinScore)
					{
						continue;
					}
					//Don't think the API has an endpoint that holds the sizes of every image?
					//if (!HasValidSize(post, out _))
					//{
					//	continue;
					//}
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
		private static async Task<ApiKey> GetApiKeyAsync(IDownloaderClient client, string username, string password)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key) && (key.CreatedAt + key.ValidFor) > DateTime.UtcNow)
			{
				return key;
			}

			var query = new Uri("https://oauth.secure.pixiv.net/auth/token");
			var result = await client.GetTextAsync(() =>
			{
				var req = client.GenerateReq(query, HttpMethod.Post);
				req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					{ "get_secure_url", "1" },
					{ "client_id", _ClientId },
					{ "client_secret", _ClientSecret },
					{ "grant_type", "password" },
					{ "username", username },
					{ "password", password },
				});
				return req;
			}).CAF();
			if (!result.IsSuccess)
			{
				throw new InvalidOperationException("Unable to login to Pixiv.");
			}

			var jObj = JObject.Parse(result.Value);
			var accessToken = jObj["response"]["access_token"].ToObject<string>();
			var expiresIn = TimeSpan.FromSeconds(jObj["response"]["expires_in"].ToObject<int>());
			return (client.ApiKeys[_Type] = new ApiKey(accessToken, expiresIn));
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetPixivImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString().Replace("_d", "");
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			if (!(HttpUtility.ParseQueryString(url.Query)["illust_id"] is string id))
			{
				return ImageResponse.FromUrl(url);
			}
			var mangaQuery = new Uri($"https://www.pixiv.net/member_illust.php?mode=manga&illust_id={id}");
			var mangaResult = await client.GetHtmlAsync(() => client.GenerateReq(mangaQuery)).CAF();
			if (mangaResult.IsSuccess)
			{
				//18+ filter
				if (mangaResult.Value.DocumentNode.Descendants("p").Any(x => x.HasClass("title") && x.InnerText.Contains("R-18")))
				{
					return ImageResponse.FromException(url, new InvalidOperationException("Locked behind R18 filter."));
				}

				var div = mangaResult.Value.DocumentNode.Descendants("div");
				var itemContainer = div.Where(x => x.GetAttributeValue("class", "") == "item-container");
				var images = itemContainer.Select(x => x.Descendants("img").Single());
				var imageUrls = images.Select(x => new Uri(FixPixivUrl(x.GetAttributeValue("data-src", ""))));
				return ImageResponse.FromImages(imageUrls);
			}
			var mediumQuery = new Uri($"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={id}");
			var mediumResult = await client.GetHtmlAsync(() => client.GenerateReq(mediumQuery)).CAF();
			if (mediumResult.IsSuccess)
			{
				//18+ filter
				if (mediumResult.Value.DocumentNode.Descendants("p").Any(x => x.HasClass("title") && x.InnerText.Contains("R-18")))
				{
					return ImageResponse.FromException(url, new InvalidOperationException("Locked behind R18 filter."));
				}

				var div = mediumResult.Value.DocumentNode.Descendants("div");
				var imgContainer = div.Single(x => x.GetAttributeValue("class", "") == "img-container");
				var img = imgContainer.Descendants("img").Single();
				var imageUrl = new Uri(FixPixivUrl(img.GetAttributeValue("src", "")));
				return ImageResponse.FromUrl(imageUrl);
			}
			return ImageResponse.FromNotFound(url);
		}
		/// <summary>
		/// Removes arguments that make the picture smaller.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private static string FixPixivUrl(string url)
		{
			//Converts top to bottom. If already in bottom format, then just return that
			//https://i.pximg.net/img-master/img/2018/04/05/17/15/28/68087246_p0_master1200.png
			//https://i.pximg.net/img-original/img/2018/04/05/17/15/28/68087246_p0.png
			if (url.Contains("img-original"))
			{
				return url;
			}
			var pMatch = _FindP.Match(url);
			if (pMatch.Index > -1)
			{
				url = $"{url.Substring(0, pMatch.Index + pMatch.Length - 1)}{Path.GetExtension(url)}";
			}
			return _RemoveC.Replace(url, "/").Replace("img-master", "img-original");
		}
	}
}