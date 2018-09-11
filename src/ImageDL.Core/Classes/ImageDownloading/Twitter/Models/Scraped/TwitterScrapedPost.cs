using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped
{
	/// <summary>
	/// Json model for a post from Twitter.
	/// </summary>
	public sealed class TwitterScrapedPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.twitter.com/{Username}/status/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => FavoriteCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <summary>
		/// The urls of the images in the tweet.
		/// </summary>
		[JsonProperty("image_urls")]
		public IList<Uri> ImageUrls { get; private set; }
		/// <summary>
		/// The name of the user who posted this.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when this was created.
		/// </summary>
		[JsonProperty("created_at_timestamp")]
		public ulong CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The amount of likes the post has.
		/// </summary>
		[JsonProperty("favorite_count")]
		public int FavoriteCount { get; private set; }
		/// <summary>
		/// The amount of retweets the post has.
		/// </summary>
		[JsonProperty("retweet_count")]
		public int RetweetCount { get; private set; }
		/// <summary>
		/// The amount of comments the post has.
		/// </summary>
		[JsonProperty("comment_count")]
		public int CommentCount { get; private set; }
		/// <summary>
		/// Whether this post is a reply.
		/// </summary>
		[JsonProperty("is_reply")]
		public bool IsReply { get; private set; }
		/// <summary>
		/// Whether this post is a retweet.
		/// </summary>
		[JsonProperty("is_retweet")]
		public bool IsRetweet { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="TwitterScrapedPost"/>.
		/// </summary>
		/// <param name="node"></param>
		public TwitterScrapedPost(HtmlNode node)
		{
			var div = node.Descendants("div");
			var span = node.Descendants("span");

			var tweetInfo = div.SingleOrDefault(x =>
			{
				var attr = x.GetAttributeValue("class", "");
				return attr.Contains("js-stream-tweet") || attr.Contains("js-original-tweet");
			});
			if (tweetInfo == null)
			{
				return;
			}
			Id = tweetInfo.GetAttributeValue("data-tweet-id", null);
			Username = tweetInfo.GetAttributeValue("data-screen-name", null);

			var media = div.SingleOrDefault(x => x.GetAttributeValue("class", "").Contains("AdaptiveMedia-container"));
			ImageUrls = (media?.Descendants("img")?.Select(x => x.GetAttributeValue("src", null)) ?? new string[0])
				.Where(x => x != null)
				.Select(x => new Uri(x)).ToList();

			CreatedAtTimestamp = (ulong)span.Single(x => x.GetAttributeValue("class", "").Contains("js-short-timestamp"))
				.GetAttributeValue("data-time", -1);
			FavoriteCount = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--favorite"))
				.Descendants("span").Select(x => x.GetAttributeValue("data-tweet-stat-count", -1)).Max();
			RetweetCount = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--retweet"))
				.Descendants("span").Select(x => x.GetAttributeValue("data-tweet-stat-count", -1)).Max();
			CommentCount = span.Single(x => x.GetAttributeValue("class", "").Contains("ProfileTweet-action--reply"))
				.Descendants("span").Select(x => x.GetAttributeValue("data-tweet-stat-count", -1)).Max();

			IsReply = div.Any(x => x.GetAttributeValue("class", null) == "ReplyingToContextBelowAuthor");
			IsRetweet = span.Any(x => x.GetAttributeValue("class", null) == "js-retweet-text");
		}

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromImages(ImageUrls));
		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}
