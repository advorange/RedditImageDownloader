using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Weibo.Models.WeiboPost;

namespace ImageDL.Classes.ImageDownloading.Weibo
{
	/// <summary>
	/// Downloads images from Weibo.
	/// </summary>
	[DownloaderName("Weibo")]
	public sealed class WeiboPostDownloader : PostDownloader
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
		/// Creates an instance of <see cref="WeiboPostDownloader"/>
		/// </summary>
		public WeiboPostDownloader()
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
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 10); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://m.weibo.cn/api/container/getIndex" +
					$"?containerid=230413{userId}_-_longbloglist" +
					$"&page={i + 1}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JObject.Parse(result.Value)["data"]["cards"].Select(x => x["mblog"].ToObject<Model>()).ToList();
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
					if (post.Pictures == null)
					{
						continue;
					}
					//Remove all images that are too small to be downloaded
					foreach (var picture in post.Pictures.Where(x => !HasValidSize(x.Large.Geo, out _)).ToList())
					{
						post.Pictures.Remove(picture);
					}
					if (!post.Pictures.Any())
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
		/// Gets the id of the Weibo user.
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

			var query = new Uri($"https://s.weibo.com/user/{WebUtility.UrlEncode(username)}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Weibo user id.");
			}

			try
			{
				var a = result.Value.DocumentNode.Descendants("a");
				var users = a.Where(x => x.GetAttributeValue("uid", null) != null);
				var user = users.First(x => x.GetAttributeValue("title", "").CaseInsEquals(username));
				return Convert.ToUInt64(user.GetAttributeValue("uid", null));
			}
			catch (Exception e)
			{
				throw new HttpRequestException("Unable to get the Weibo user id.", e);
			}
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetWeiboPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"https://m.weibo.cn/api/statuses/show?id={id}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<Model>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetWeiboImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			//Url is formatted something like this https://weibo.com/1632765501/GijfWif2d
			var parts = url.LocalPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2 && await GetWeiboPostAsync(client, parts[1]).CAF() is Model post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}