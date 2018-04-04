#pragma warning disable 1591, 649
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about media. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public sealed class MediaNode : Post
	{
		[JsonProperty("__typename")]
		public readonly string Typename;
		[JsonProperty("shortcode")]
		public readonly string Shortcode;
		[JsonProperty("dimensions")]
		public readonly Dimensions Dimensions;
		[JsonProperty("gating_info")]
		public readonly object GatingInfo;
		[JsonProperty("media_preview")]
		public readonly object MediaPreview;
		[JsonProperty("display_resources")]
		public readonly List<Thumbnail> Thumbnails;
		[JsonProperty("is_video")]
		public readonly bool IsVideo;
		[JsonProperty("should_log_client_event")]
		public readonly bool ShouldLogClientEvent;
		[JsonProperty("tracking_token")]
		public readonly string TrackingToken;
		[JsonProperty("edge_media_to_tagged_user")]
		public readonly TaggedUserInfo TaggedUserInfo;
		[JsonProperty("edge_media_to_caption")]
		public readonly CaptionInfo CaptionInfo;
		[JsonProperty("caption_is_edited")]
		public readonly bool CaptionIsEdited;
		[JsonProperty("edge_media_to_comment")]
		public readonly CommentInfo CommentInfo;
		[JsonProperty("comments_disabled")]
		public readonly bool CommentsDisabled;
		[JsonProperty("taken_at_timestamp")]
		public readonly long TakenAtTimestamp;
		[JsonProperty("edge_media_preview_like")]
		public readonly LikeInfo LikeInfo;
		[JsonProperty("edge_media_to_sponsor_user")]
		public readonly SponsorInfo SponsorInfo;
		[JsonProperty("location")]
		public readonly Location Location;
		[JsonProperty("viewer_has_liked")]
		public readonly bool ViewerHasLiked;
		[JsonProperty("viewer_has_saved")]
		public readonly bool ViewerHasSaved;
		[JsonProperty("viewer_has_saved_to_collection")]
		public readonly bool ViewerHasSavedToCollection;
		[JsonProperty("owner")]
		public readonly User Owner;
		[JsonProperty("is_ad")]
		public readonly bool IsAd;
		[JsonProperty("edge_web_media_to_related_media")]
		public readonly RelatedMediaInfo RelatedMediaInfo;
		[JsonProperty("edge_sidecar_to_children")]
		public readonly ChildrenInfo ChildrenInfo;

		[JsonProperty("id")]
		private readonly string _Id;
		[JsonProperty("display_url")]
		private readonly string _DisplayUrl;

		/// <inheritdoc />
		[JsonIgnore]
		public override string PostUrl => $"https://www.instagram.com/p/{Shortcode}";
		/// <inheritdoc />
		[JsonIgnore]
		public override string ContentUrl => _DisplayUrl;
		/// <inheritdoc />
		[JsonIgnore]
		public override string Id => _Id;
		/// <inheritdoc />
		[JsonIgnore]
		public override int Score => LikeInfo.Count;
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(TakenAtTimestamp)).ToUniversalTime();
		[JsonIgnore]
		public bool HasChildren => Typename == "GraphSidecar";

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Shortcode} ({Typename})";
		}
	}
}
