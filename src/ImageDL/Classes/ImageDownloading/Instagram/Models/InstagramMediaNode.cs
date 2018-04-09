using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about media. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public sealed class InstagramMediaNode : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.instagram.com/p/{Shortcode}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => LikeInfo.Count;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(TakenAtTimestamp)).ToUniversalTime();
		/// <summary>
		/// The type of post this is. Video, single image, multi image, etc.
		/// </summary>
		[JsonProperty("__typename")]
		public string Typename { get; private set; }
		/// <summary>
		/// The shortcode to this post.
		/// </summary>
		[JsonProperty("shortcode")]
		public string Shortcode { get; private set; }
		/// <summary>
		/// The size of the image.
		/// </summary>
		[JsonProperty("dimensions")]
		public InstagramImageDimensions Dimensions { get; private set; }
		/// <summary>
		/// Not sure what this is, not even age gated accounts have this field.
		/// </summary>
		[JsonProperty("gating_info")]
		public object GatingInfo { get; private set; }
		/// <summary>
		/// A long string of gibberish if the post is a video.
		/// </summary>
		[JsonProperty("media_preview")]
		public string MediaPreview { get; private set; }
		/// <summary>
		/// The thumbnails of each image in the post.
		/// </summary>
		[JsonProperty("display_resources")]
		public IList<InstagramThumbnail> Thumbnails { get; private set; }
		/// <summary>
		/// The url to the video if there is one.
		/// </summary>
		[JsonProperty("video_url")]
		public Uri VideoUrl { get; private set; }
		/// <summary>
		/// The amount of views a video has, if there is one.
		/// </summary>
		[JsonProperty("video_view_count")]
		public int VideoViewCount { get; private set; }
		/// <summary>
		/// Whether or not the post is a video.
		/// </summary>
		[JsonProperty("is_video")]
		public bool IsVideo { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("should_log_client_event")]
		public bool ShouldLogClientEvent { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("tracking_token")]
		public string TrackingToken { get; private set; }
		/// <summary>
		/// The users who were mentioned in a post.
		/// </summary>
		[JsonProperty("edge_media_to_tagged_user")]
		public InstagramTaggedUserInfo TaggedUserInfo { get; private set; }
		/// <summary>
		/// The captions on images.
		/// </summary>
		[JsonProperty("edge_media_to_caption")]
		public InstagramCaptionInfo CaptionInfo { get; private set; }
		/// <summary>
		/// If the caption has been edited.
		/// </summary>
		[JsonProperty("caption_is_edited")]
		public bool CaptionIsEdited { get; private set; }
		/// <summary>
		/// How many comments the post has, and who commented on it.
		/// </summary>
		[JsonProperty("edge_media_to_comment")]
		public InstagramCommentInfo CommentInfo { get; private set; }
		/// <summary>
		/// Whether or not comments are disabled on this post.
		/// </summary>
		[JsonProperty("comments_disabled")]
		public bool CommentsDisabled { get; private set; }
		/// <summary>
		/// How many likes the post has, and who liked it.
		/// </summary>
		[JsonProperty("edge_media_preview_like")]
		public InstagramLikeInfo LikeInfo { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("edge_media_to_sponsor_user")]
		public InstagramSponsorInfo SponsorInfo { get; private set; }
		/// <summary>
		/// The location the photos were taken at.
		/// </summary>
		[JsonProperty("location")]
		public InstagramLocation Location { get; private set; }
		/// <summary>
		/// If you have liked the post. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_liked")]
		public bool ViewerHasLiked { get; private set; }
		/// <summary>
		/// If you have saved the post. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_saved")]
		public bool ViewerHasSaved { get; private set; }
		/// <summary>
		/// If you have saved the post to your collection. This will always be false since we're not logged in.
		/// </summary>
		[JsonProperty("viewer_has_saved_to_collection")]
		public bool ViewerHasSavedToCollection { get; private set; }
		/// <summary>
		/// Who posted the post.
		/// </summary>
		[JsonProperty("owner")]
		public InstagramUser Owner { get; private set; }
		/// <summary>
		/// Whether or not this post is an advertisement.
		/// </summary>
		[JsonProperty("is_ad")]
		public bool IsAd { get; private set; }
		/// <summary>
		/// Related media, usually empty so not sure what it actually stores.
		/// </summary>
		[JsonProperty("edge_web_media_to_related_media")]
		public InstagramRelatedMediaInfo RelatedMediaInfo { get; private set; }
		/// <summary>
		/// The children of the post.
		/// </summary>
		[JsonProperty("edge_sidecar_to_children")]
		public InstagramChildrenInfo ChildrenInfo { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when the post was taken.
		/// </summary>
		[JsonProperty("taken_at_timestamp")]
		public long TakenAtTimestamp { get; private set; }
		/// <summary>
		/// The link to the post.
		/// </summary>
		[JsonProperty("display_url")]
		public Uri DisplayUrl { get; private set; }

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			if (ChildrenInfo.Nodes != null)
			{
				var tasks = ChildrenInfo.Nodes.Select(async x => await x.Child.GetImagesAsync(client).CAF());
				var urls = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x.ImageUrls).ToArray();
				return ImageResponse.FromImages(urls);
			}
			return VideoUrl != null
				? ImageResponse.FromAnimated(VideoUrl)
				: ImageResponse.FromUrl(PostUrl);
		}
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