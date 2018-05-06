using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth;
using ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter
{
	/// <summary>
	/// Downloads images from Twitter.
	/// </summary>
	[DownloaderName("Twitter")]
	public sealed class TwitterImageDownloader : ImageDownloader
	{
		//Source file: https://abs.twimg.com/k/en/init.en.9fe6ea43287284f537d7.js
		private static readonly string _Token = "AAAAAAAAAAAAAAAAAAAAAPYXBAAAAAAACLXUNDekMxqa8h%2F40K4moUkGsoc%3DTYfbDKbT3jJPCEVnMYqilB28NHfOPqkca3qaAxGfsyKCs0wRbw";

		/// <summary>
		/// The name of the user to download images from.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="TwitterImageDownloader"/>.
		/// </summary>
		public TwitterImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var parsed = new TwitterScrapedPage();
			//Iterate to update the pagination start point.
			for (string min = ""; list.Count < AmountOfPostsToGather && (min == "" || parsed.HasMoreItems); min = parsed.MinimumPosition)
			{
				var query = $"https://twitter.com/i/profiles/show/{Username}/media_timeline" +
					$"?include_available_features=1" +
					$"&include_entities=1" +
					$"&reset_error_state=false" +
					$"&count=100";
				if (min != "")
				{
					query += $"&max_position={min}";
				}
				var result = await client.GetTextAsync(() => client.GenerateReq(new Uri(query)), TimeSpan.FromSeconds(60)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JsonConvert.DeserializeObject<TwitterScrapedPage>(result.Value);
				foreach (var post in parsed.Items)
				{
					//Could grab each full post (has media sizes and such) but that only has 1000 reqs per 15 mins
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.FavoriteCount < MinScore)
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
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<TwitterOAuthPost> GetTwitterPostAsync(IImageDownloaderClient client, string id)
		{
			var query = new Uri($"https://api.twitter.com/1.1/statuses/show.json?id={id}");
			var result = await client.GetTextAsync(() =>
			{
				var req = client.GenerateReq(query);
				req.Headers.Add("Authorization", $"Bearer {_Token}");
				return req;
			}, TimeSpan.FromMinutes(15)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<TwitterOAuthPost>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetTwitterImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/status/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetTwitterPostAsync(client, id).CAF() is TwitterOAuthPost post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}
