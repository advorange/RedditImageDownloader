using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for a Tumlbr post.
	/// </summary>
	public class TumblrPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonProperty("url")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonProperty("note-count")]
		public int Score { get; private set; } = -1;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(UnixTimestamp)).ToUniversalTime();
		/// <summary>
		/// The full url.
		/// </summary>
		[JsonProperty("url-with-slug")]
		public Uri UrlWithSlug { get; private set; }
		/// <summary>
		/// The type of post.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// Indicates whether this was created from Tumblr bookmarklet.
		/// </summary>
		[JsonProperty("bookmarklet")]
		public int Bookmarklet { get; private set; }
		/// <summary>
		/// Indicates whether this was created from mobile.
		/// </summary>
		[JsonProperty("mobile")]
		public int Mobile { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("feed-item")]
		public string FeedItem { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("from-feed-id")]
		public int FromFeedId { get; private set; }
		/// <summary>
		/// Post format, html or markdown.
		/// </summary>
		[JsonProperty("format")]
		public string Format { get; private set; }
		/// <summary>
		/// How to reblog the post via the api.
		/// </summary>
		[JsonProperty("reblog-key")]
		public string ReblogKey { get; private set; }
		/// <summary>
		/// The extra unnecessary info from the full url.
		/// </summary>
		[JsonProperty("slug")]
		public string Slug { get; private set; }
		/// <summary>
		/// Whether this is the original post.
		/// </summary>
		[JsonProperty("is-submission")]
		public bool IsSubmission { get; private set; }
		/// <summary>
		/// Html element for the button to like.
		/// </summary>
		[JsonProperty("like-button")]
		public string LikeButton { get; private set; }
		/// <summary>
		/// Html element for the button to reblog.
		/// </summary>
		[JsonProperty("reblog-button")]
		public string ReblogButton { get; private set; }
		/// <summary>
		/// Link to the post this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-url")]
		public Uri RebloggedFromUrl { get; private set; }
		/// <summary>
		/// Name of the subdomain this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-name")]
		public string RebloggedFromName { get; private set; }
		/// <summary>
		/// Title of the subdomain this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged-from-title")]
		public string RebloggedFromTitle { get; private set; }
		/// <summary>
		/// Profile picture of the person this was reblogged from.
		/// </summary>
		[JsonProperty("reblogged_from_avatar_url_512")]
		public Uri RebloggedFromAvatarUrl { get; private set; }
		/// <summary>
		/// Link to the post this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-url")]
		public Uri RebloggedRootUrl { get; private set; }
		/// <summary>
		/// Name of the subdomain this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-name")]
		public string RebloggedRootName { get; private set; }
		/// <summary>
		/// Title of the subdomain this was originally from.
		/// </summary>
		[JsonProperty("reblogged-root-title")]
		public string RebloggedRootTitle { get; private set; }
		/// <summary>
		/// Profile picture of the person this was originally from.
		/// </summary>
		[JsonProperty("reblogged_root_avatar_url_512")]
		public Uri RebloggedRootAvatarUrl { get; private set; }
		/// <summary>
		/// The owner of the post.
		/// </summary>
		[JsonProperty("tumblelog")]
		public TumblrPostOwner Tumblelog { get; private set; }
		/// <summary>
		/// The caption of the post.
		/// </summary>
		[JsonProperty("photo-caption")]
		public string PhotoCaption { get; private set; }
		/// <summary>
		/// The source of the photo.
		/// </summary>
		[JsonProperty("photo-link-url")]
		public Uri PhotoLinkUrl { get; private set; }
		/// <summary>
		/// The width of the photo.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The height of the photo.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The photos of the post if this is a multiphoto post.
		/// </summary>
		[JsonProperty("photos")]
		public IList<TumblrPhoto> Photos { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds for when this post was posted.
		/// </summary>
		[JsonProperty("unix-timestamp")]
		public int UnixTimestamp { get; private set; }
		/// <summary>
		/// String representing when the post was posted in gmt.
		/// </summary>
		[JsonProperty("date-gmt")]
		public string GmtDateString { get; private set; }
		/// <summary>
		/// String representing when the post was posted.
		/// </summary>
		[JsonProperty("date")]
		public string DateString { get; private set; }
		/// <summary>
		/// The link to the image.
		/// </summary>
		[JsonProperty("photo-url-1280")]
		public Uri RegularImageUrl { get; private set; }
		/// <summary>
		/// The link to the raw image.
		/// </summary>
		[JsonIgnore]
		public Uri FullSizeImageUrl => TumblrImageGatherer.GetFullSizeImage(RegularImageUrl);

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			if (Photos != null)
			{
				var urls = Photos.Select(x => x.FullSizeImageUrl).ToArray();
				return Task.FromResult(ImageResponse.FromImages(urls));
			}
			return Task.FromResult(ImageResponse.FromUrl(FullSizeImageUrl));
		}
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