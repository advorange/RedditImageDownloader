using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped
{
	/// <summary>
	/// Json model for the results of a Twitter page.
	/// </summary>
	public struct TwitterScrapedPage
	{
		/// <summary>
		/// The lowest value id of the items.
		/// </summary>
		[JsonProperty("min_position")]
		public string MinimumPosition
		{
			get => _MinimumPosition ?? ItemIds.Last();
			private set => _MinimumPosition = value;
		}
		/// <summary>
		/// Whether more items can be gotten.
		/// </summary>
		[JsonProperty("has_more_items")]
		public bool HasMoreItems { get; private set; }
		/// <summary>
		/// The html of the items.
		/// </summary>
		[JsonProperty("items_html")]
		public string ItemsHtml
		{
			get => _ItemsHtml;
			private set
			{
				_ItemsHtml = Regex.Replace(Regex.Unescape(WebUtility.HtmlDecode(value)), @"\\u(?<Value>[a-zA-Z0-9]{4})", m =>
				{
					return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
				});

				var doc = new HtmlDocument();
				doc.LoadHtml(_ItemsHtml);

				var li = doc.DocumentNode.Descendants("li");
				var tweets = li.Where(x => x.GetAttributeValue("class", "").Contains("js-stream-item"));
				Items = tweets.Select(t =>
				{
					var div = t.Descendants("div");
					var span = t.Descendants("span");

					var imageUrls = div.SingleOrDefault(x => x.GetAttributeValue("class", "").Contains("AdaptiveMedia-container"))
						?.Descendants("img")?.Select(x => x.GetAttributeValue("src", null))?.Where(x => x != null) ?? new string[0];
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
						{ nameof(TwitterScrapedPost.FavoriteCount), likes.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(TwitterScrapedPost.RetweetCount), retweets.GetAttributeValue("data-tweet-stat-count", -1) },
						{ nameof(TwitterScrapedPost.CommentCount), replies.GetAttributeValue("data-tweet-stat-count", -1) },
					}.ToObject<TwitterScrapedPost>();
				}).ToList();
				ItemIds = Items.Select(x => x.Id).ToList();
			}
		}
		/// <summary>
		/// The html converted to objects.
		/// </summary>
		[JsonIgnore]
		public IList<string> ItemIds { get; private set; }
		/// <summary>
		/// The posts.
		/// </summary>
		[JsonIgnore]
		public IList<TwitterScrapedPost> Items { get; private set; }
		/// <summary>
		/// The amount of posts gotten.
		/// </summary>
		[JsonProperty("new_latent_count")]
		public int NewLatentCount { get; private set; }
		[JsonIgnore]
		private string _ItemsHtml;
		[JsonIgnore]
		private string _MinimumPosition;
	}
}