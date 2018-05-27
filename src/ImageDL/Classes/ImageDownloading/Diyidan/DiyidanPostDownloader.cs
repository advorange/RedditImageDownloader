using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.Diyidan.Models.DiyidanPost;

namespace ImageDL.Classes.ImageDownloading.Diyidan
{
	/// <summary>
	/// Downloads images from Diyidan.
	/// </summary>
	[DownloaderName("Diyidan")]
	public sealed class DiyidanPostDownloader : PostDownloader
	{
		/// <summary>
		/// The id of the user to search for.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="DiyidanPostDownloader"/>.
		/// </summary>
		public DiyidanPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name or id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = await GetUserIdAsync(client, Username).CAF();
			var parsed = new List<Model>();
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://www.diyidan.com/user/posts/{userId}/{i}");
				var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var li = result.Value.DocumentNode.Descendants("li");
				var images = li.Where(x => x.GetAttributeValue("data-post_id", null) != null);
				parsed = images.Select(x => new Model(x)).ToList();
				foreach (var post in parsed)
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
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
		/// <summary>
		/// Gets the id of the Diyidan user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<ulong> GetUserIdAsync(IDownloaderClient client, string username)
		{
			if (ulong.TryParse(username, out var val))
			{
				return val;
			}

			var query = new Uri($"https://www.diyidan.com/search/1?keyword={username}&search_type=user&post_type=综合");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Diyidan user id.");
			}

			try
			{
				var li = result.Value.DocumentNode.Descendants("li");
				var users = li.Where(x => x.GetAttributeValue("class", null) == "search_users_item users_item_");
				var user = users.Single(x =>
				{
					var usernameElement = x.Descendants("em").Single(y => y.GetAttributeValue("class", null) == "k_highlight");
					return usernameElement?.InnerText == username;
				});
				var followingElement = user.Descendants("div").Single(x => x.GetAttributeValue("class", null) == "user_following_box");
				return Convert.ToUInt64(followingElement.GetAttributeValue("data-id", null));
			}
			catch (Exception e)
			{
				throw new HttpRequestException("Unable to get the Diyidan user id.", e);
			}
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetDiyidanImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var result = await client.GetHtmlAsync(() => client.GenerateReq(url)).CAF();
			if (!result.IsSuccess)
			{
				return ImageResponse.FromNotFound(url);
			}
			var div = result.Value.DocumentNode.Descendants("div");
			if (div.Any(x => x.GetAttributeValue("class", null) == "video_404_box"))
			{
				return ImageResponse.FromAnimated(url);
			}
			var content = div.SingleOrDefault(x => x.GetAttributeValue("class", "").CaseInsContains("user_post_content"));
			if (content == null)
			{
				return ImageResponse.FromNotFound(url);
			}
			var img = content.Descendants("img");
			var src = img.Select(x => x.GetAttributeValue("src", ""));
			var urls = src.Select(x => new Uri($"https:{x.Substring(0, x.LastIndexOf('!'))}"));
			return src.Any() ? ImageResponse.FromImages(urls) : ImageResponse.FromNotFound(url);
		}
	}
}