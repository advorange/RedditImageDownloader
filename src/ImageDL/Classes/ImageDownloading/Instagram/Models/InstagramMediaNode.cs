using System;
using System.Collections.Generic;
using System.Linq;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about media. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public sealed class InstagramMediaNode : IPost
	{
		#region Json
		/// <summary>
		/// The type of post this is. Video, single image, multi image, etc.
		/// </summary>
		[JsonProperty("__typename")]
		public readonly string Typename;
		/// <summary>
		/// The shortcode to this post.
		/// </summary>
		[JsonProperty("shortcode")]
		public readonly string Shortcode;
		/// <summary>
		/// The size of the image.
		/// </summary>
		[JsonProperty("dimensions")]
		public readonly InstagramImageDimensions Dimensions;
		/// <summary>
		/// Not sure what this is, not even age gated accounts have this field.
		/// </summary>
		[JsonProperty("gating_info")]
		public readonly object GatingInfo;
		/// <summary>
		/// A long string of gibberish if the post is a video.
		/// </summary>
		[JsonProperty("media_preview")]
		public readonly string MediaPreview;
		/// <summary>
		/// The thumbnails of each image in the post.
		/// </summary>
		[JsonProperty("display_resources")]
		public readonly List<InstagramThumbnail> Thumbnails;
		/// <summary>
		/// Whether or not the post is a video.
		/// </summary>
		[JsonProperty("is_video")]
		public readonly bool IsVideo;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("should_log_client_event")]
		public readonly bool ShouldLogClientEvent;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("tracking_token")]
		public readonly string TrackingToken;
		/// <summary>
		/// The users who were mentioned in a post.
		/// </summary>
		[JsonProperty("edge_media_to_tagged_user")]
		public readonly InstagramTaggedUserInfo TaggedUserInfo;
		/// <summary>
		/// The captions on images.
		/// </summary>
		[JsonProperty("edge_media_to_caption")]
		public readonly InstagramCaptionInfo CaptionInfo;
		/// <summary>
		/// If the caption has been edited.
		/// </summary>
		[JsonProperty("caption_is_edited")]
		public readonly bool CaptionIsEdited;
		/// <summary>
		/// How many comments the post has, and who commented on it.
		/// </summary>
		[JsonProperty("edge_media_to_comment")]
		public readonly InstagramCommentInfo CommentInfo;
		/// <summary>
		/// Whether or not comments are disabled on this post.
		/// </summary>
		[JsonProperty("comments_disabled")]
		public readonly bool CommentsDisabled;
		/// <summary>
		/// How many likes the post has, and who liked it.
		/// </summary>
		[JsonProperty("edge_media_preview_like")]
		public readonly InstagramLikeInfo LikeInfo;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("edge_media_to_sponsor_user")]
		public readonly InstagramSponsorInfo SponsorInfo;
		/// <summary>
		/// The location the photos were taken at.
		/// </summary>
		[JsonProperty("location")]
		public readonly InstagramLocation Location;
		/// <summary>
		/// If you have liked the post. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_liked")]
		public readonly bool ViewerHasLiked;
		/// <summary>
		/// If you have saved the post. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_saved")]
		public readonly bool ViewerHasSaved;
		/// <summary>
		/// If you have saved the post to your collection. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_saved_to_collection")]
		public readonly bool ViewerHasSavedToCollection;
		/// <summary>
		/// Who posted the post.
		/// </summary>
		[JsonProperty("owner")]
		public readonly InstagramUser Owner;
		/// <summary>
		/// Whether or not this post is an advertisement.
		/// </summary>
		[JsonProperty("is_ad")]
		public readonly bool IsAd;
		/// <summary>
		/// Related media, usually empty so not sure what it actually stores.
		/// </summary>
		[JsonProperty("edge_web_media_to_related_media")]
		public readonly InstagramRelatedMediaInfo RelatedMediaInfo;
		/// <summary>
		/// The children of the post.
		/// </summary>
		[JsonProperty("edge_sidecar_to_children")]
		public readonly InstagramChildrenInfo ChildrenInfo;
		/// <summary>
		/// The unix timestamp in seconds of when the post was taken.
		/// </summary>
		[JsonProperty("taken_at_timestamp")]
		public readonly long TakenAtTimestamp;
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		private readonly string _Id = null;
		/// <summary>
		/// The link to the post.
		/// </summary>
		[JsonProperty("display_url")]
		private readonly string _DisplayUrl = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri($"https://www.instagram.com/p/{Shortcode}");
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => ChildrenInfo.Nodes?.SelectMany(x => x.Child.ContentUrls) ?? new[] { new Uri(_DisplayUrl) };
		/// <inheritdoc />
		public int Score => LikeInfo.Count;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(TakenAtTimestamp)).ToUniversalTime();

		/// <summary>
		/// Returns the shortcode and type name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Shortcode} ({Typename})";
		}
	}
}