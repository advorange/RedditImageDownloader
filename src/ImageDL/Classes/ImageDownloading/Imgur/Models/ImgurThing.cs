using System;
using System.Collections.Generic;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for a post/image from Imgur.
	/// </summary>
	public abstract class ImgurThing : IPost
	{
		#region Json
		/// <summary>
		/// Title of the post.
		/// </summary>
		[JsonProperty("title")]
		public readonly string Title;
		/// <summary>
		/// Description of the post.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;
		/// <summary>
		/// How many people have viewed the post.
		/// </summary>
		[JsonProperty("views")]
		public readonly int Views;
		/// <summary>
		/// How you have voted. Will always be null since we're using an anonymous api key.
		/// </summary>
		[JsonProperty("vote")]
		public readonly string Vote;
		/// <summary>
		/// The amount of upvotes people have given.
		/// </summary>
		[JsonProperty("ups")]
		public readonly int? UpScore;
		/// <summary>
		/// The amount of downvotes people have given.
		/// </summary>
		[JsonProperty("downs")]
		public readonly int? DownScore;
		/// <summary>
		/// The displayed score of a post.
		/// </summary>
		[JsonProperty("points")]
		public readonly int? Points;
		/// <summary>
		/// How many comments are on a post.
		/// </summary>
		[JsonProperty("comment_count")]
		public readonly int? CommentCount;
		/// <summary>
		/// How many people have favorited a post.
		/// </summary>
		[JsonProperty("favorite_count")]
		public readonly int? FavoriteCount;
		/// <summary>
		/// Tags on an image for topics related to it.
		/// </summary>
		[JsonProperty("tags")]
		public readonly List<ImgurTag> Tags;
		/// <summary>
		/// Whether or not this post is an advertisement.
		/// </summary>
		[JsonProperty("is_ad")]
		public readonly bool IsAd;
		/// <summary>
		/// The type of ad it is, if it is an advertisement.
		/// </summary>
		[JsonProperty("ad_type")]
		public readonly int AdType;
		/// <summary>
		/// The url to the ad.
		/// </summary>
		[JsonProperty("ad_url")]
		public readonly string AdUrl;
		/// <summary>
		/// Whether or not this is on the front page of imgur.
		/// </summary>
		[JsonProperty("in_most_viral")]
		public readonly bool IsInMostViral;
		/// <summary>
		/// The name of whoever posted the post.
		/// </summary>
		[JsonProperty("account_url")]
		public readonly string AccountName;
		/// <summary>
		/// The id of whoever posted the post.
		/// </summary>
		[JsonProperty("account_id")]
		public readonly int? AccountId;
		/// <summary>
		/// If you have favorited the post. Will always be false since we're using an anonymous api key.
		/// </summary>
		[JsonProperty("favorite")]
		public readonly bool IsFavorited;
		/// <summary>
		/// If the post is NSFW, e.g. nudity.
		/// </summary>
		[JsonProperty("nsfw")]
		public readonly bool? IsNSFW;
		/// <summary>
		/// The subreddit this post was posted to.
		/// </summary>
		[JsonProperty("section")]
		public readonly string Section;
		/// <summary>
		/// The size of the mp4 file if this is an mp4.
		/// </summary>
		[JsonProperty("mp4_size")]
		public readonly long Mp4Size;
		/// <summary>
		/// Whether or not to loop if this is an mp4.
		/// </summary>
		[JsonProperty("looping")]
		public readonly bool IsLooping;
		/// <summary>
		/// The link to the mp4 file if this is an mp4.
		/// </summary>
		[JsonProperty("mp4")]
		public readonly string Mp4Link;
		/// <summary>
		/// The link to the gifv, which is a wrapper of <see cref="Mp4Link"/>.
		/// </summary>
		[JsonProperty("gifv")]
		public readonly string GifvLink;
		/// <summary>
		/// The unix timestamp in seconds of when the post was created.
		/// </summary>
		[JsonProperty("datetime")]
		public readonly long DateTimeTimestamp;
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		private readonly string _Id = null;
		/// <summary>
		/// The link to the post.
		/// </summary>
		[JsonProperty("link")]
		private readonly string _Link = null;
		/// <summary>
		/// The score of the post.
		/// </summary>
		[JsonProperty("score")]
		private readonly int? _Score = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri(_Link);
		/// <inheritdoc />
		public abstract IEnumerable<Uri> ContentUrls { get; }
		/// <inheritdoc />
		public int Score => _Score ?? -1;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(DateTimeTimestamp)).ToUniversalTime();

		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}