using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth;
using ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Twitter
{
	/// <summary>
	/// Downloads images from Twitter.
	/// </summary>
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
		public TwitterImageDownloader() : base("Twitter")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			await GetPostsThroughRegularApi(client, list).CAF();
		}
		/// <summary>
		/// Gets posts through the api that is regularly used on the webpage.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		private async Task GetPostsThroughRegularApi(IImageDownloaderClient client, List<IPost> list)
		{
			var parsed = new List<TwitterScrapedPost>();
			//Iterate to update the pagination start point.
			for (var min = ""; list.Count < AmountOfPostsToGather && (min == "" || parsed.Count >= 20); min = parsed.Last().Id)
			{
				var query = $"https://twitter.com/i/profiles/show/{Username}/media_timeline" +
					$"?include_available_features=1" +
					$"&include_entities=1" +
					$"&reset_error_state=false";
				if (min != "")
				{
					query += $"&max_position={min}";
				}
				var result = await client.GetText(client.GetReq(new Uri(query))).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var html = WebUtility.HtmlDecode(JObject.Parse(result.Value)["items_html"].ToString());
				var unicodeHtml = Regex.Replace(Regex.Unescape(html), @"\\u(?<Value>[a-zA-Z0-9]{4})", m =>
				{
					return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
				});

				var doc = new HtmlDocument();
				doc.LoadHtml(unicodeHtml);

				var li = doc.DocumentNode.Descendants("li");
				var tweets = li.Where(x => x.GetAttributeValue("class", "").Contains("js-stream-item"));
				parsed = tweets.Select(t =>
				{
					var div = t.Descendants("div");
					var span = t.Descendants("span");

					var imageUrls = div.SingleOrDefault(x => x.GetAttributeValue("class", "").Contains("AdaptiveMedia-container"))
						.Descendants("img").Select(x => x.GetAttributeValue("src", null)).Where(x => x != null);
					var tweetInfo = div.Single(x => x.GetAttributeValue("class", "").Contains("js-stream-tweet"));
					var timestamp = span.Single(x => x.GetAttributeValue("class", "").Contains("js-short-timestamp"));
					var likes = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--favorite"))
						.Descendants("span").First();
					var retweets = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--retweet"))
						.Descendants("span").First();
					var replies = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--reply"))
						.Descendants("span").First();

					return new JObject
					{
						{ nameof(TwitterScrapedPost.ImageUrls), JArray.FromObject(imageUrls) },
						{ nameof(TwitterScrapedPost.Id), tweetInfo.GetAttributeValue("data-tweet-id", null) },
						{ nameof(TwitterScrapedPost.Username), tweetInfo.GetAttributeValue("data-screen-name", null) },
						{ nameof(TwitterScrapedPost.CreatedAtTimestamp), timestamp.GetAttributeValue("data-time", 0) },
						{ nameof(TwitterScrapedPost.LikeCount), likes.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(TwitterScrapedPost.RetweetCount), retweets.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(TwitterScrapedPost.CommentCount), replies.GetAttributeValue("data-tweet-stat-count", -1) },
					}.ToObject<TwitterScrapedPost>();
				}).ToList();

				foreach (var post in parsed)
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					else if (post.LikeCount < MinScore)
					{
						continue;
					}
					else if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
		/// <summary>
		/// Gets posts through the OAuth api using a key from a public Javascript file.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		private Task GetPostsThroughOAuthApi(IImageDownloader client, List<IPost> list)
		{
			//Honestly, this is more annoying than just getting the HTML from the JSON
			//The only benefit is that this contains the size of the media content
			//But it has a big drawback of only being able to access the 3200 most recent posts.
			throw new NotImplementedException();
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
			var req = client.GetReq(query);
			req.Headers.Add("Authorization", $"Bearer {_Token}");
			var result = await client.GetText(req).CAF();
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
				var id = u.Substring(index + search.Length).Split('/')[0];
				if (await GetTwitterPostAsync(client, id).CAF() is IPost post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}
