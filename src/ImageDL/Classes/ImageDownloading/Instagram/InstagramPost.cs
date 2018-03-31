using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
#pragma warning disable 1591 //Disabled since most of these are self explanatory and this is a glorified Json model
	/// <summary>
	/// Json model for the results from gathering posts from Instagram.
	/// </summary>
	public sealed class InstagramResult
	{
		[JsonProperty("count")]
		public readonly int Count;
		[JsonProperty("page_info")]
		public readonly PageInfo PageInfo;
		[JsonProperty("edges")]
		public readonly List<InstagramPostNode> Nodes;
	}

	/// <summary>
	/// Holds the pagination information.
	/// </summary>
	public struct PageInfo
	{
		[JsonProperty("has_next_page")]
		public readonly bool HasNextPage;
		[JsonProperty("end_cursor")]
		public readonly string EndCursor;
	}

	/// <summary>
	/// Holds an Instagram post.
	/// </summary>
	public sealed class InstagramPostNode
	{
		[JsonProperty("node")]
		public readonly InstagramPost Post;
	}

	/// <summary>
	/// Json model for a post from Instagram
	/// </summary>
	public sealed class InstagramPost
	{
		[JsonProperty("id")]
		public readonly ulong Id;
		[JsonProperty("__typename")]
		public readonly string TypeName;
		[JsonProperty("edge_media_to_caption")]
		public readonly Caption Caption;
		[JsonProperty("shortcode")]
		public readonly string Shortcode;
		[JsonProperty("edge_media_to_comment")]
		public readonly CommentInfo CommentInfo;
		[JsonProperty("comments_disabled")]
		public readonly bool CommentsDisabled;
		[JsonProperty("taken_at_timestamp")]
		public readonly long TakenAtTimestamp;
		[JsonProperty("dimensions")]
		public readonly Dimensions Dimensions;
		[JsonProperty("display_url")]
		public readonly string DisplayUrl;
		[JsonProperty("edge_media_preview_like")]
		public readonly LikeInfo LikeInfo;
		[JsonProperty("gating_info")]
		public readonly string GatingInfo;
		[JsonProperty("media_preview")]
		public readonly string MediaPreview;
		[JsonProperty("owner")]
		public readonly OwnerInfo OwnerInfo;
		[JsonProperty("thumbnail_src")]
		public readonly string ThumanilSource;
		[JsonProperty("thumbnail_resources")]
		public readonly List<Thumbnail> ThumbnailResources;
		[JsonProperty("is_video")]
		public readonly bool IsVideo;

		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(TakenAtTimestamp)).ToUniversalTime();
	}

	/// <summary>
	/// Holds the text on an image.
	/// </summary>
	public struct Caption
	{
		[JsonProperty("edges")]
		public readonly List<CaptionEdge> Edges;
	}

	/// <summary>
	/// Holds the caption node of a caption.
	/// </summary>
	public struct CaptionEdge
	{
		[JsonProperty("node")]
		public readonly CaptionNode Node;
	}

	/// <summary>
	/// Holds the text of a caption.
	/// </summary>
	public struct CaptionNode
	{
		[JsonProperty("text")]
		public readonly string Text;
	}

	/// <summary>
	/// Holds information about the comments on the post.
	/// </summary>
	public struct CommentInfo
	{
		[JsonProperty("count")]
		public readonly int Count;
	}

	/// <summary>
	/// Holds the dimensions of the image.
	/// </summary>
	public struct Dimensions
	{
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("width")]
		public readonly int Width;
	}

	/// <summary>
	/// Holds information about the likes on the post.
	/// </summary>
	public struct LikeInfo
	{
		[JsonProperty("count")]
		public readonly int Count;
	}

	/// <summary>
	/// Holds information about the owner of the post.
	/// </summary>
	public struct OwnerInfo
	{
		[JsonProperty("id")]
		public readonly ulong Id;
	}

	/// <summary>
	/// Holds information about a thumbnail.
	/// </summary>
	public struct Thumbnail
	{
		[JsonProperty("src")]
		public readonly string Source;
		[JsonProperty("config_width")]
		public readonly int Width;
		[JsonProperty("config_height")]
		public readonly int Height;
	}
#pragma warning restore 1591
}