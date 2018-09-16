using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser.Implementation.Instance;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql;
using ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Instagram.Models.InstagramMediaNode;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Downloads images from Instagram.
	/// </summary>
	[DownloaderName("Instagram")]
	public sealed class InstagramPostDownloader : PostDownloader
	{
		private static readonly Type _Type = typeof(InstagramPostDownloader);

		/// <summary>
		/// The name of the user to search for.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="InstagramPostDownloader"/>.
		/// </summary>
		public InstagramPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Username, new[] { "user" })
			{
				Description = "The name or id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var (id, rhx) = await GetUserIdAndRhxAsync(client, Username).CAF();
			var parsed = new InstagramMediaTimeline();
			//Iterate to update the pagination start point
			for (string end = ""; list.Count < AmountOfPostsToGather && (end == "" || parsed.PageInfo.HasNextPage); end = parsed.PageInfo.EndCursor)
			{
				token.ThrowIfCancellationRequested();
				var variables = JsonConvert.SerializeObject(new Dictionary<string, object>
				{
					{ "id", id }, //The id of the user to search
					{ "first", 50 }, //The amount of posts to get. Max allowed is 50, any other and bad request error is gotten
					{ "after", end }, //The position in the pagination. Empty/null just returns the start of the user's images
				});
				var query = new Uri("https://www.instagram.com/graphql/query/" +
					$"?query_hash={await GetApiKeyAsync(client).CAF()}" +
					$"&variables={WebUtility.UrlEncode(variables)}");
				var result = await client.GetTextAsync(() => GenerateApiReq(client, query, rhx, variables)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the query hash, try to get another one
					if (result.Value.Contains("query_hash"))
					{
						//Means the query hash cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						continue;
					}
					return;
				}

				parsed = JsonConvert.DeserializeObject<InstagramResult>(result.Value).Data.User.Content;
				foreach (var post in parsed.Posts)
				{
					token.ThrowIfCancellationRequested();
					if (post.Node.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Node.Score < MinScore)
					{
						continue;
					}
					var p = await GetInstagramPostAsync(client, post.Node.Shortcode).CAF();
					if (p.ChildrenInfo.Nodes != null && p.ChildrenInfo.Nodes.Any()) //Only if children exist
					{
						foreach (var node in p.ChildrenInfo.Nodes.Where(x => !HasValidSize(x.Child.Dimensions, out _)).ToList())
						{
							p.ChildrenInfo.Nodes.Remove(node);
						}
						if (!p.ChildrenInfo.Nodes.Any())
						{
							continue;
						}
					}
					else if (!HasValidSize(p.Dimensions, out _)) //Otherwise check dimensions directly
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
		private static async Task<ApiKey> GetApiKeyAsync(IDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}

			//Load the page regularly first so we can get some data from it
			var query = new Uri($"https://www.instagram.com/instagram/?hl=en");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the user's account.");
			}

			//Find the direct link to ProfilePageContainer.js
			var jsLink = result.Value.DocumentNode.Descendants("link")
				.Select(x => x.GetAttributeValue("href", ""))
				.First(x => x.Contains("ProfilePageContainer.js"));
			var jsQuery = new Uri($"https://www.instagram.com{jsLink}");
			var jsResult = await client.GetTextAsync(() => client.GenerateReq(jsQuery)).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the query hash.");
			}

			//Read ProfilePageContainer.js and find the query id
			//There are multiple query ids in this file, and Instagram likes changing the location of each valid one,
			//so we have to do some searching with long strings and regex
			//(o=e.profilePosts.byUserId.get(t))||void 0===o?void 0:o.pagination},queryId:\"
			//(n=e.profilePosts.byUserId.get(t))||void 0===n?void 0:n.pagination},queryId:\"
			var qSearch = @"\([a-zA-Z]=e\.profilePosts\.byUserId\.get\(t\)\)\|\|void 0===[a-zA-Z]\?void 0:[a-zA-Z]\.pagination},queryId:""";
			var qMatch = Regex.Matches(jsResult.Value, qSearch).Cast<Match>().Single();
			var qCut = jsResult.Value.Substring(qMatch.Index + qMatch.Length);
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
		private static HttpRequestMessage GenerateApiReq(IDownloaderClient client, Uri url, string rhx, string variables)
		{
			var gis = "";
			using (var md5 = MD5.Create())
			{
				//Magic string, likely to change in the future
				var magic = $"{rhx}:{variables}";
				gis = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(magic))).Replace("-", "").ToLower();
			}

			//Not sure what GIS stands for, but it's what Instagram calls it
			var req = client.GenerateReq(url);
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
		private static async Task<(ulong Id, string Rhx)> GetUserIdAndRhxAsync(IDownloaderClient client, string username)
		{
			var query = new Uri($"https://www.instagram.com/{username}/?hl=en");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the user's account.");
			}
			if (result.Value.Contains("You must be 18 years old or over to see this profile"))
			{
				throw new HttpRequestException("Unable to access this profile due to age restrictions.");
			}

			var idSearch = "profilePage_";
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
		public static async Task<Model> GetInstagramPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"https://www.instagram.com/p/{id}/?__a=1");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<InstagramGraphqlResult>(result.Value).Graphql.ShortcodeMedia : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetInstagramImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/p/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetInstagramPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}