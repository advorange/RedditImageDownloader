﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for a post/image from Imgur.
	/// </summary>
	public abstract class ImgurThing : IPost
	{
		/// <summary>
		/// The id of whoever posted the post.
		/// </summary>
		[JsonProperty("account_id")]
		public readonly int? AccountId;

		/// <summary>
		/// The name of whoever posted the post.
		/// </summary>
		[JsonProperty("account_url")]
		public readonly string AccountName;

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
		/// How many comments are on a post.
		/// </summary>
		[JsonProperty("comment_count")]
		public readonly int? CommentCount;

		/// <summary>
		/// The unix timestamp in seconds of when the post was created.
		/// </summary>
		[JsonProperty("datetime")]
		public readonly long DateTimeTimestamp;

		/// <summary>
		/// Description of the post.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;

		/// <summary>
		/// The amount of downvotes people have given.
		/// </summary>
		[JsonProperty("downs")]
		public readonly int? DownScore;

		/// <summary>
		/// How many people have favorited a post.
		/// </summary>
		[JsonProperty("favorite_count")]
		public readonly int? FavoriteCount;

		/// <summary>
		/// The link to the gifv, which is a wrapper of <see cref="Mp4Url"/>.
		/// </summary>
		[JsonProperty("gifv")]
		public readonly Uri GifvUrl;

		/// <summary>
		/// Whether or not this post is an advertisement.
		/// </summary>
		[JsonProperty("is_ad")]
		public readonly bool IsAd;

		/// <summary>
		/// If you have favorited the post. Will always be false since we're using an anonymous api key.
		/// </summary>
		[JsonProperty("favorite")]
		public readonly bool IsFavorited;

		/// <summary>
		/// Whether or not this is on the front page of imgur.
		/// </summary>
		[JsonProperty("in_most_viral")]
		public readonly bool IsInMostViral;

		/// <summary>
		/// Whether or not to loop if this is an mp4.
		/// </summary>
		[JsonProperty("looping")]
		public readonly bool IsLooping;

		/// <summary>
		/// If the post is NSFW, e.g. nudity.
		/// </summary>
		[JsonProperty("nsfw")]
		public readonly bool? IsNSFW;

		/// <summary>
		/// The size of the mp4 file if this is an mp4.
		/// </summary>
		[JsonProperty("mp4_size")]
		public readonly long Mp4Size;

		/// <summary>
		/// The link to the mp4 file if this is an mp4.
		/// </summary>
		[JsonProperty("mp4")]
		public readonly Uri Mp4Url;

		/// <summary>
		/// The displayed score of a post.
		/// </summary>
		[JsonProperty("points")]
		public readonly int? Points;

		/// <summary>
		/// The subreddit this post was posted to.
		/// </summary>
		[JsonProperty("section")]
		public readonly string Section;

		/// <summary>
		/// Tags on an image for topics related to it.
		/// </summary>
		[JsonProperty("tags")]
		public readonly List<ImgurTag> Tags;

		/// <summary>
		/// Title of the post.
		/// </summary>
		[JsonProperty("title")]
		public readonly string Title;

		/// <summary>
		/// The amount of upvotes people have given.
		/// </summary>
		[JsonProperty("ups")]
		public readonly int? UpScore;

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

		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(DateTimeTimestamp)).ToUniversalTime();

		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <inheritdoc />
		[JsonProperty("link")]
		public Uri PostUrl { get; private set; }

		/// <inheritdoc />
		[JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
		public int Score { get; private set; } = -1;

		/// <inheritdoc />
		public abstract Task<ImageResponse> GetImagesAsync(IDownloaderClient client);

		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}