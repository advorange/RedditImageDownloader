using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Twitter.Models.TwitterPost;

namespace ImageDL.Classes.ImageDownloading.Twitter
{
	/// <summary>
	/// Downloads images from Twitter.
	/// </summary>
	public sealed class TwitterImageDownloader : ImageDownloader
	{
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
			var parsed = new List<Model>();
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
						{ nameof(Model.ImageUrls), JArray.FromObject(imageUrls) },
						{ nameof(Model.Id), tweetInfo.GetAttributeValue("data-tweet-id", null) },
						{ nameof(Model.Username), tweetInfo.GetAttributeValue("data-screen-name", null) },
						{ nameof(Model.CreatedAtTimestamp), timestamp.GetAttributeValue("data-time", 0) },
						{ nameof(Model.LikeCount), likes.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(Model.RetweetCount), retweets.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(Model.CommentCount), replies.GetAttributeValue("data-tweet-stat-count", -1) },
					}.ToObject<Model>();
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
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetTwitterPostAsync(IImageDownloaderClient client, string id)
		{
			var query = new Uri($"https://twitter.com/i/web/status/{id}");
			var result = await client.GetHtml(client.GetReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}
			return null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetTwitterImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return null;
		}
	}
}
