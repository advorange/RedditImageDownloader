using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using ImageDL.Classes.SettingParsing;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Downloads images from Instagram.
	/// </summary>
	public class InstagramImageDownloader : ImageDownloader<InstagramPost>
	{
		/// <summary>
		/// The name of the user to search for.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="InstagramImageDownloader"/>.
		/// </summary>
		public InstagramImageDownloader() : base("Instagram")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<InstagramPost> list)
		{
			var userId = 0UL;
			var parsed = new InstagramResult();
			var keepGoing = true;
			//Iterate to update the pagination start point
			for (var nextPage = ""; keepGoing && list.Count < AmountOfPostsToGather && (nextPage == "" || parsed.PageInfo.HasNextPage); nextPage = parsed.PageInfo.EndCursor)
			{
				//If the id is 0 either this just started or it was reset due to the key becoming invalid
				if (userId == 0)
				{
					var (QueryHash, UserId) = await GetQueryHashAndUserId().CAF();
					if (QueryHash == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.UpdateAPIKey(QueryHash, TimeSpan.FromDays(1));
					userId = UserId;
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateQuery(userId, Client.APIKey, nextPage)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the query hash, try to get another one
					if (result.Text.Contains("query_hash"))
					{
						userId = 0;
						continue;
					}
					break;
				}

				parsed = JObject.Parse(result.Text)["data"]["user"]["edge_owner_to_timeline_media"].ToObject<InstagramResult>();
				foreach (var node in parsed.Nodes)
				{
					if (!(keepGoing = node.Post.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (!FitsSizeRequirements(null, node.Post.Dimensions.Width, node.Post.Dimensions.Height, out _)
						|| node.Post.LikeInfo.Count < MinScore)
					{
						continue;
					}
					else if (!(keepGoing = Add(list, node.Post)))
					{
						break;
					}
				}
			}
		}
		/// <inheritdoc />
		protected override List<InstagramPost> OrderAndRemoveDuplicates(List<InstagramPost> list)
		{
			return list.OrderByDescending(x => x.LikeInfo.Count).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(InstagramPost post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.LikeInfo.Count}] https://www.instagram.com/p/{post.Shortcode}/");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(InstagramPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(InstagramPost post)
		{
			return await Client.ScrapeImagesAsync(new Uri(post.DisplayUrl)).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(InstagramPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.LikeInfo.Count, reason);
		}

		private Uri GenerateQuery(ulong userId, string queryHash, string nextPagination)
		{
			var variables = JsonConvert.SerializeObject(new Dictionary<string, object>
			{
				{ "id", userId }, //The id of the user to search
				{ "first", 12 }, //The amount of posts to get?
				{ "after", nextPagination ?? "" }, //The position in the pagination
			});
			return new Uri("https://www.instagram.com/graphql/query/" +
				$"?query_hash={queryHash}" +
				$"&variables={WebUtility.UrlEncode(variables)}");
		}
		private async Task<(string QueryHash, ulong UserId)> GetQueryHashAndUserId()
		{
			//Load the page regularly first so we can get some data from it
			var fLink = $"https://www.instagram.com/{Username}/?hl=en";
			var fResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(fLink)).CAF();
			if (!fResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the first request to the user's account.");
			}

			//Put it in a doc so we can actually parse it
			var doc = new HtmlDocument();
			doc.LoadHtml(fResult.Text);

			//Since we already used the text, we can now cut it to get the userid
			var idSearch = "owner\":{\"id\":\"";
			var idCut = fResult.Text.Substring(fResult.Text.IndexOf(idSearch) + idSearch.Length);
			var id = idCut.Substring(0, idCut.IndexOf('"'));

			//Find the direct link to ProfilePageContainer.js
			var jsLink = doc.DocumentNode.Descendants("link")
				.Select(x => x.GetAttributeValue("href", null))
				.First(x => (x ?? "").Contains("ProfilePageContainer.js"));
			var jsResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri($"https://www.instagram.com{jsLink}")).CAF();
			if (!jsResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the request to the Javascript holding the query hash.");
			}

			//Read ProfilePageContainer.js and find the query hash
			var querySearch = "e.profilePosts.byUserId.get(t))?o.pagination:o},queryId:\"";
			var queryCut = jsResult.Text.Substring(jsResult.Text.IndexOf(querySearch) + querySearch.Length);
			var queryHash = queryCut.Substring(0, queryCut.IndexOf('"'));

			return (queryHash, Convert.ToUInt64(id));
		}
	}
}