using ImageDL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for a Tumlbr post.
	/// </summary>
	public class TumblrPost : IPost
	{
		#region Json
		/// <summary>
		/// The full url.
		/// </summary>
		[JsonProperty("url-with-slug")]
		public readonly string UrlWithSlug;
		/// <summary>
		/// The type of post.
		/// </summary>
		[JsonProperty("type")]
		public readonly string Type;
		/// <summary>
		/// Indicates whether this was created from Tumblr bookmarklet.
		/// </summary>
		[JsonProperty("bookmarklet")]
		public readonly int Bookmarklet;
		/// <summary>
		/// Indicates whether this was created from mobile.
		/// </summary>
		[JsonProperty("mobile")]
		public readonly int Mobile;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("feed-item")]
		public readonly string FeedItem;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("from-feed-id")]
		public readonly int FromFeedId;
		/// <summary>
		/// Post format, html or markdown.
		/// </summary>
		[JsonProperty("format")]
		public readonly string Format;
		/// <summary>
		/// How to reblog the post via the api.
		/// </summary>
		[JsonProperty("reblog-key")]
		public readonly string ReblogKey;
		/// <summary>
		/// The extra unnecessary info from the full url.
		/// </summary>
		[JsonProperty("slug")]
		public readonly string Slug;
		/// <summary>
		/// Whether this is the original post.
		/// </summary>
		[JsonProperty("is-submission")]
		public readonly bool IsSubmission;
		/// <summary>
		/// Html element for the button to like.
		/// </summary>
		[JsonProperty("like-button")]
		public readonly string LikeButton;
		/// <summary>
		/// Html element for the button to reblog.
		/// </summary>
		[JsonProperty("reblog-button")]
		public readonly string ReblogButton;
		/// <summary>
		/// Link to the post this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-url")]
		public readonly string RebloggedFromUrl;
		/// <summary>
		/// Name of the subdomain this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-name")]
		public readonly string RebloggedFromName;
		/// <summary>
		/// Title of the subdomain this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-title")]
		public readonly string RebloggedFromTitle;
		/// <summary>
		/// Profile picture of the person this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged_from_avatar_url_512")]
		public readonly string RebloggedFromAvatarUrl;
		/// <summary>
		/// Link to the post this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-url")]
		public readonly string RebloggedRootUrl;
		/// <summary>
		/// Name of the subdomain this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-name")]
		public readonly string RebloggedRootName;
		/// <summary>
		/// Title of the subdomain this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-title")]
		public readonly string RebloggedRootTitle;
		/// <summary>
		/// Profile picture of the person this was originally from.
		/// </summary>
		[JsonProperty("reblogged_root_avatar_url_512")]
		public readonly string RebloggedRootAvatarUrl;
		/// <summary>
		/// The owner of the post.
		/// </summary>
		[JsonProperty("tumblelog")]
		public readonly TumblrPostOwner Tumblelog;
		/// <summary>
		/// The caption of the post.
		/// </summary>
		[JsonProperty("photo-caption")]
		public readonly string PhotoCaption;
		/// <summary>
		/// The source of the photo.
		/// </summary>
		[JsonProperty("photo-link-url")]
		public readonly string PhotoLinkUrl;
		/// <summary>
		/// The width of the photo.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The height of the photo.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The photos of the post if this is a multiphoto post.
		/// </summary>
		[JsonProperty("photos")]
		public readonly List<TumblrPhoto> Photos;
		/// <summary>
		/// The unix timestamp in seconds for when this post was posted.
		/// </summary>
		[JsonProperty("unix-timestamp")]
		public readonly int UnixTimestamp;
		/// <summary>
		/// String representing when the post was posted in gmt.
		/// </summary>
		[JsonProperty("date-gmt")]
		public readonly string GmtDateString;
		/// <summary>
		/// String representing when the post was posted.
		/// </summary>
		[JsonProperty("date")]
		public readonly string DateString;
		/// <summary>
		/// How many notes the post has gotten.
		/// </summary>
		[JsonProperty("note-count")]
		private readonly int _NoteCount = -1;
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		private readonly string _Id = null;
		/// <summary>
		/// The url leading to the post.
		/// </summary>
		[JsonProperty("url")]
		private readonly string _Url = null;
		/// <summary>
		/// The link to the image.
		/// </summary>
		[JsonProperty("photo-url-1280")]
		private readonly string _PhotoUrl = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri(_Url);
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => Photos?.Select(x => x.PhotoUrl)
			?? new[] { TumblrImageGatherer.GetFullSizeImage(new Uri(_PhotoUrl)) };
		/// <inheritdoc />
		public int Score => _NoteCount;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(UnixTimestamp)).ToUniversalTime();
	}
}