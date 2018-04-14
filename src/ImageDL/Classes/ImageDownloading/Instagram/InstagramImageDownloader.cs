using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql;
using ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Instagram.Models.InstagramMediaNode;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Downloads images from Instagram.
	/// </summary>
	public sealed class InstagramImageDownloader : ImageDownloader
	{
		private static readonly Type _Type = typeof(InstagramImageDownloader);

		/// <summary>
		/// The name of the user to search for.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}
		/// <summary>
		/// The id of the user to seach for.
		/// </summary>
		public ulong UserId
		{
			get => _UserId;
			set => _UserId = value;
		}

		private string _Username;
		private ulong _UserId;

		/// <summary>
		/// Creates an instance of <see cref="InstagramImageDownloader"/>.
		/// </summary>
		public InstagramImageDownloader() : base("Instagram")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to search for.",
			});
			SettingParser.Add(new Setting<ulong>(new[] { nameof(UserId), "id" }, x => UserId = x)
			{
				Description = "The id of the user to search for. This should only be used if the account is age restricted because the downloader can't get the id.",
				IsOptional = true,
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var id = 0UL;
			var rhx = "";
			var parsed = new InstagramMediaTimeline();
			//Iterate to update the pagination start point
			for (var nextPage = ""; list.Count < AmountOfPostsToGather && (nextPage == "" || parsed.PageInfo.HasNextPage); nextPage = parsed.PageInfo.EndCursor)
			{
				//If the id is 0 either this just started or it was reset due to the key becoming invalid
				if (id == 0UL)
				{
					var (Id, Rhx) = await GetUserIdAndRhx(client, Username).CAF();
					id = Id;
					rhx = Rhx;
				}

				var variables = JsonConvert.SerializeObject(new Dictionary<string, object>
				{
					{ "id", id }, //The id of the user to search
					{ "first", 100 }, //The amount of posts to get
					{ "after", nextPage ?? "" }, //The position in the pagination
				});
				var query = new Uri("https://www.instagram.com/graphql/query/" +
					$"?query_hash={await GetApiKeyAsync(client).CAF()}" +
					$"&variables={WebUtility.UrlEncode(variables)}");
				var result = await client.GetText(GenerateApiReq(client, query, rhx, variables)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the query hash, try to get another one
					if (result.Value.Contains("query_hash"))
					{
						//Means the access token cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						continue;
					}
					return;
				}

				var insta = JsonConvert.DeserializeObject<InstagramResult>(result.Value);
				foreach (var post in (parsed = insta.Data.User.Content).Posts)
				{
					var p = await GetInstagramPostAsync(client, post.Node.Shortcode).CAF();
					if (p.CreatedAt < OldestAllowed)
					{
						return;
					}
					else if (p.LikeInfo.Count < MinScore)
					{
						continue;
					}
					else if (p.ChildrenInfo.Nodes?.Any() ?? false) //Only go into this statment if there are any children
					{
						foreach (var node in p.ChildrenInfo.Nodes.Where(x => !HasValidSize(null, x.Child.Dimensions.Width, x.Child.Dimensions.Height, out _)).ToList())
						{
							p.ChildrenInfo.Nodes.Remove(node);
						}
						if (!p.ChildrenInfo.Nodes.Any())
						{
							continue;
						}
					}
					//If there are no children, we can check the post's dimensions directly
					else if (!HasValidSize(null, p.Dimensions.Width, p.Dimensions.Height, out _))
					{
						continue;
					}
					if (!Add(list, p))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// Gets an api key for Instagram.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}

			//Load the page regularly first so we can get some data from it
			var query = new Uri($"https://www.instagram.com/instagram/?hl=en");
			var result = await client.GetHtml(client.GetReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the user's account.");
			}

			//Find the direct link to ProfilePageContainer.js
			var jsLink = result.Value.DocumentNode.Descendants("link")
				.Select(x => x.GetAttributeValue("href", null))
				.First(x => (x ?? "").Contains("ProfilePageContainer.js"));
			var jsQuery = new Uri($"https://www.instagram.com{jsLink}");
			var jsResult = await client.GetText(client.GetReq(jsQuery)).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the query hash.");
			}

			//Read ProfilePageContainer.js and find the query hash
			var qSearch = "e.profilePosts.byUserId.get(t))?o.pagination:o},queryId:\"";
			var qCut = jsResult.Value.Substring(jsResult.Value.IndexOf(qSearch) + qSearch.Length);
			return (client.ApiKeys[_Type] = new ApiKey(qCut.Substring(0, qCut.IndexOf('"'))));
		}
		/// <summary>
		/// Gets the gis code for this request.
		/// </summary>
		/// <param name="client">The client being used to download images.</param>
		/// <param name="url">The webpage being gathered from.</param>
		/// <param name="rhx">A variable on the webpage.</param>
		/// <param name="variables">The search terms in the query parameters.</param>
		/// <returns></returns>
		private static HttpRequestMessage GenerateApiReq(IImageDownloaderClient client, Uri url, string rhx, string variables)
		{
			//Magic string, likely to change in the future
			var text = $"{rhx}:{client.Cookies.GetCookies(url)["csrftoken"].Value}:{variables}";
			var gis = "";
			using (var md5 = MD5.Create())
			{
				gis = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(text))).Replace("-", "").ToLower();
			}

			//Not sure what GIS stands for, but it's what Instagram calls it
			var req = client.GetReq(url);
			req.Headers.Add("X-Instagram-GIS", gis);
			req.Headers.Add("X-Requested-With", "XMLHttpRequest");
			return req;
		}
		/// <summary>
		/// Gets the id of the Instagram user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<(ulong Id, string Rhx)> GetUserIdAndRhx(IImageDownloaderClient client, string username)
		{
			var query = new Uri($"https://www.instagram.com/{username}/?hl=en");
			var result = await client.GetText(client.GetReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the user's account.");
			}
			if (result.Value.Contains("You must be 18 years old or over to see this profile"))
			{
				throw new HttpRequestException("Unable to access this profile due to age restrictions.");
			}

			var idSearch = "owner\":{\"id\":\"";
			var idCut = result.Value.Substring(result.Value.IndexOf(idSearch) + idSearch.Length);
			var id = Convert.ToUInt64(idCut.Substring(0, idCut.IndexOf('"')));

			var rhxSearch = "\"rhx_gis\":\"";
			var rhxCut = result.Value.Substring(result.Value.IndexOf(rhxSearch) + rhxSearch.Length);
			var rhx = rhxCut.Substring(0, rhxCut.IndexOf('"'));
			return (id, rhx);
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetInstagramPostAsync(IImageDownloaderClient client, string id)
		{
			var query = new Uri($"https://www.instagram.com/p/{id}/?__a=1");
			var result = await client.GetText(client.GetReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<InstagramGraphqlResult>(result.Value).Graphql.ShortcodeMedia;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetInstagramImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/p/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/')[0];
				if (await GetInstagramPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}