using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Bcy.Models.BcyPost;

namespace ImageDL.Classes.ImageDownloading.Bcy
{
	/// <summary>
	/// Downloads images from Bcy.
	/// </summary>
	[DownloaderName("Bcy")]
	public sealed class BcyImageDownloader : ImageDownloader
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
		/// Creates an instance of <see cref="BcyImageDownloader"/>
		/// </summary>
		public BcyImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name or id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var userId = await GetUserIdAsync(client, Username).CAF();
			var parsed = new List<Model>();
			//Iterate becasue there's a limit of 20 per page
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); ++i)
			{
				var data = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					{ "uid", userId.ToString() },
					{ "since", (i * 20).ToString() },
					{ "limit", 20.ToString() },
					{ "filter", "origin" }, //Specify to only look for posts from the user
					{ "source", "all" }, //Could specify "drawer" or "coser" here, but we don't know which one to search for
				});
				var query = new Uri("https://bcy.net/home/user/loadtimeline");
				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(query, HttpMethod.Post);
					req.Content = data;
					req.Headers.Add("X-Requested-With", "XMLHttpRequest");
					return req;
				}).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JObject.Parse(result.Value)["data"].ToObject<List<Model>>();
				foreach (var post in parsed)
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					//First check indicates the post doesn't have any images
					//Due to the way this site's api is set up, we can't get all images and check ahead of time
					if (post.Details.FirstImage.Width == 0 || post.Score < MinScore)
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
		/// Gets the id of the Bcy user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<ulong> GetUserIdAsync(IImageDownloaderClient client, string username)
		{
			if (ulong.TryParse(username, out var val))
			{
				return val;
			}

			var query = new Uri($"https://bcy.net/search/user?k={username}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the Bcy user id.");
			}

			try
			{
				var a = result.Value.DocumentNode.Descendants("a");
				var user = a.Single(x => x.GetAttributeValue("title", "").CaseInsEquals(username));
				return Convert.ToUInt64(user.GetAttributeValue("href", null).Replace("/u/", ""));
			}
			catch (Exception e)
			{
				throw new HttpRequestException("Unable to get the Bcy user id.", e);
			}
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetBcyImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var result = await client.GetHtmlAsync(() => client.GenerateReq(url)).CAF();
			if (!result.IsSuccess)
			{
				return ImageResponse.FromNotFound(url);
			}
			var img = result.Value.DocumentNode.Descendants("img");
			var details = img.Where(x => x.GetAttributeValue("class", "").CaseInsContains("detail_std"));
			var src = details.Select(x => x.GetAttributeValue("src", ""));
			var urls = src.Select(x => new Uri(x.Substring(0, x.LastIndexOf('/')).Replace("img5", "img9")));
			return src.Any() ? ImageResponse.FromImages(urls) : ImageResponse.FromNotFound(url);
		}
	}
}