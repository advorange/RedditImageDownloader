using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql;
using ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Instagram.Models.MediaNode;

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
		protected override async Task GatherPostsAsync(List<Model> list)
		{
			var userId = 0UL;
			var parsed = new MediaTimeline();
			var keepGoing = true;
			//Iterate to update the pagination start point
			for (var nextPage = ""; keepGoing && list.Count < AmountOfPostsToGather && (nextPage == "" || parsed.PageInfo.HasNextPage); nextPage = parsed.PageInfo.EndCursor)
			{
				//If the id is 0 either this just started or it was reset due to the key becoming invalid
				if (String.IsNullOrWhiteSpace(Client[Name]))
				{
					var (QueryHash, UserId) = await GetQueryHashAndUserId().CAF();
					if (QueryHash == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.ApiKeys[Name] = new ApiKey(QueryHash);
					userId = UserId;
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateQuery(userId, Client[Name], nextPage)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the query hash, try to get another one
					if (result.Text.Contains("query_hash"))
					{
						Client.ApiKeys[Name] = new ApiKey(null);
						continue;
					}
					break;
				}

				var insta = JsonConvert.DeserializeObject<InstagramResult>(result.Text);
				foreach (var post in (parsed = insta.Data.User.Content).Posts)
				{
					var p = await Parse(post.Node).CAF();
					if (!(keepGoing = p.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (p.LikeInfo.Count < MinScore)
					{
						continue;
					}
					else if (p.HasChildren)
					{
						//Remove all images that don't meet the size requirements
						for (int j = p.ChildrenInfo.Nodes.Count - 1; j >= 0; --j)
						{
							var image = p.ChildrenInfo.Nodes[j].Child;
							if (!FitsSizeRequirements(null, image.Dimensions.Width, image.Dimensions.Height, out _))
							{
								p.ChildrenInfo.Nodes.RemoveAt(j);
							}
						}
						if (!p.ChildrenInfo.Nodes.Any())
						{
							continue;
						}
					}
					//If there are no children, we can check the post's dimensions directly
					else if (!FitsSizeRequirements(null, p.Dimensions.Width, p.Dimensions.Height, out _))
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
		/// <inheritdoc />
		protected override List<Model> OrderAndRemoveDuplicates(List<Model> list)
		{
			return list.OrderByDescending(x => x.LikeInfo.Count).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(Model post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.LikeInfo.Count}] https://www.instagram.com/p/{post.Shortcode}/");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(Model post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override Task<ScrapeResult> GatherImagesAsync(Model post)
		{
			var postUrl = new Uri($"https://www.instagram.com/p/{post.Shortcode}");
			var images = post.HasChildren
				? post.ChildrenInfo.Nodes.Select(x => new Uri(x.Child.DisplayUrl))
				: new[] { new Uri(post.DisplayUrl) };
			return Task.FromResult(new ScrapeResult(postUrl, false, new InstagramScraper(), images, null));
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(Model post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.LikeInfo.Count, reason);
		}

		private Uri GenerateQuery(ulong userId, string queryHash, string nextPagination)
		{
			var variables = JsonConvert.SerializeObject(new Dictionary<string, object>
			{
				{ "id", userId }, //The id of the user to search
				{ "first", 100 }, //The amount of posts to get
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
		private async Task<Model> Parse(Model post)
		{
			var query = $"https://www.instagram.com/p/{post.Shortcode}/?__a=1";
			var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException($"Unable to get all the metadata for {post.Id}.");
			}

			return JsonConvert.DeserializeObject<GraphqlResult>(result.Text).Graphql.ShortcodeMedia;
		}
	}
}