using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
	public class InstagramImageDownloader : ImageDownloader<Model>
	{
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
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			var userId = 0UL;
			var parsed = new InstagramMediaTimeline();
			var keepGoing = true;
			//Iterate to update the pagination start point
			for (var nextPage = ""; keepGoing && list.Count < AmountOfPostsToGather && (nextPage == "" || parsed.PageInfo.HasNextPage); nextPage = parsed.PageInfo.EndCursor)
			{
				//If the id is 0 either this just started or it was reset due to the key becoming invalid
				if (userId == 0UL)
				{
					userId = await GetUserIdAsync(client, Username).CAF();
				}

				var variables = JsonConvert.SerializeObject(new Dictionary<string, object>
				{
					{ "id", userId }, //The id of the user to search
					{ "first", 100 }, //The amount of posts to get
					{ "after", nextPage ?? "" }, //The position in the pagination
				});
				var query = new Uri("https://www.instagram.com/graphql/query/" +
					$"?query_hash={await GetApiKeyAsync(client).CAF()}" +
					$"&variables={WebUtility.UrlEncode(variables)}");
				var result = await client.GetText(client.GetReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the query hash, try to get another one
					if (result.Value.Contains("query_hash"))
					{
						client.ApiKeys.Remove(typeof(InstagramImageDownloader));
						continue;
					}
					break;
				}

				var insta = JsonConvert.DeserializeObject<InstagramResult>(result.Value);
				foreach (var post in (parsed = insta.Data.User.Content).Posts)
				{
					var p = await GetInstagramPostAsync(client, post.Node.Shortcode).CAF();
					if (!(keepGoing = p.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (p.LikeInfo.Count < MinScore)
					{
						continue;
					}
					else if (p.ChildrenInfo.Nodes?.Any() ?? false) //Only go into this statment if there are any children
					{
						p.ChildrenInfo.Nodes.RemoveAll(x => !HasValidSize(null, x.Child.Dimensions.Width, x.Child.Dimensions.Height, out _));
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
					if (!(keepGoing = Add(list, p)))
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets an api key for Instagram.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(typeof(InstagramImageDownloader), out var key))
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
			return (client.ApiKeys[typeof(InstagramImageDownloader)] = new ApiKey(qCut.Substring(0, qCut.IndexOf('"'))));
		}
		/// <summary>
		/// Gets the id of the Instagram user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<ulong> GetUserIdAsync(IImageDownloaderClient client, string username)
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
			return Convert.ToUInt64(idCut.Substring(0, idCut.IndexOf('"')));
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
	}
}