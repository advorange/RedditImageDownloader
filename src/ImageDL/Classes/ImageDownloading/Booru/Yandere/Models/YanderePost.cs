using System;
using System.Collections.Generic;
using ImageDL.Classes.ImageDownloading.Booru.Models;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Booru.Yandere.Models
{
	/// <summary>
	/// Json model for a Yandere post.
	/// </summary>
	public sealed class YanderePost : BooruPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public override Uri BaseUrl => new Uri("https://www.yande.re");
		/// <inheritdoc />
		[JsonIgnore]
		public override Uri PostUrl => new Uri($"{BaseUrl}post/show/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public override DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <inheritdoc />
		[JsonIgnore]
		public override int Width => _Width;
		/// <inheritdoc />
		[JsonIgnore]
		public override int Height => _Height;
		/// <inheritdoc />
		[JsonIgnore]
		public override string Tags => _Tags;
		/// <summary>
		/// The unix timestamp in seconds of when the post was created.
		/// </summary>
		[JsonProperty("created_at")]
		public int CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when the post was last updated.
		/// </summary>
		[JsonProperty("updated_at")]
		public int UpdatedAtTimestamp { get; private set; }
		/// <summary>
		/// The id of whoever created the image.
		/// </summary>
		[JsonProperty("creator_id")]
		public int? CreatorId { get; private set; }
		/// <summary>
		/// The id of whoever approved the image.
		/// </summary>
		[JsonProperty("approver_id")]
		public int? ApproverId { get; private set; }
		/// <summary>
		/// The uploader of the image.
		/// </summary>
		[JsonProperty("author")]
		public string Author { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("change")]
		public int Change { get; private set; }
		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("file_size")]
		public int FileSize { get; private set; }
		/// <summary>
		/// The extension of the image.
		/// </summary>
		[JsonProperty("file_ext")]
		public string FileExt { get; private set; }
		/// <summary>
		/// If the post is listed.
		/// </summary>
		[JsonProperty("is_shown_in_index")]
		public bool IsShownInIndex { get; private set; }
		/// <summary>
		/// The url to the 300x300 preview.
		/// </summary>
		[JsonProperty("preview_url")]
		public string PreviewUrl { get; private set; }
		/// <summary>
		/// The width of the preview scaled to 150px.
		/// </summary>
		[JsonProperty("preview_width")]
		public int PreviewWidth { get; private set; }
		/// <summary>
		/// The height of the preview scaled to 150px.
		/// </summary>
		[JsonProperty("preview_height")]
		public int PreviewHeight { get; private set; }
		/// <summary>
		/// The width of the preview.
		/// </summary>
		[JsonProperty("actual_preview_width")]
		public int ActualPreviewWidth { get; private set; }
		/// <summary>
		/// The height of the preview.
		/// </summary>
		[JsonProperty("actual_preview_height")]
		public int ActualPreviewHeight { get; private set; }
		/// <summary>
		/// Same as <see cref="JpegUrl"/> unless wider than 1500px.
		/// </summary>
		[JsonProperty("sample_url")]
		public string SampleUrl { get; private set; }
		/// <summary>
		/// The width of the sample url.
		/// </summary>
		[JsonProperty("sample_width")]
		public int SampleWidth { get; private set; }
		/// <summary>
		/// The height of the sample url.
		/// </summary>
		[JsonProperty("sample_height")]
		public int SampleHeight { get; private set; }
		/// <summary>
		/// 0 if sample url is the same as jpg url.
		/// </summary>
		[JsonProperty("sample_file_size")]
		public int SampleFileSize { get; private set; }
		/// <summary>
		/// The url if the image is a jpg. Will be scaled down to 3500px if wider.
		/// </summary>
		[JsonProperty("jpeg_url")]
		public string JpegUrl { get; private set; }
		/// <summary>
		/// Width of the jpg url file.
		/// </summary>
		[JsonProperty("jpeg_width")]
		public int JpegWidth { get; private set; }
		/// <summary>
		/// Height of the jpg url file.
		/// </summary>
		[JsonProperty("jpeg_height")]
		public int JpegHeight { get; private set; }
		/// <summary>
		/// 0 if jpg url is the same as file url.
		/// </summary>
		[JsonProperty("jpeg_file_size")]
		public int JpegFileSize { get; private set; }
		/// <summary>
		/// Whether people can change the rating.
		/// </summary>
		[JsonProperty("is_rating_locked")]
		public bool IsRatingLocked { get; private set; }
		/// <summary>
		/// The status of the image, e.g. active, etc.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; private set; }
		/// <summary>
		/// Whether the post has been approved or not.
		/// </summary>
		[JsonProperty("is_pending")]
		public bool IsPending { get; private set; }
		/// <summary>
		/// If the post has been held by a moderator.
		/// </summary>
		[JsonProperty("is_held")]
		public bool IsHeld { get; private set; }
		/// <summary>
		/// The string the pending frames are gotten from.
		/// </summary>
		[JsonProperty("frames_pending_string")]
		public string FramesPendingString { get; private set; }
		/// <summary>
		/// The pending frames of the post, for when it's a gif.
		/// </summary>
		[JsonProperty("frames_pending")]
		public IList<YandereFrame> FramesPending { get; private set; }
		/// <summary>
		/// The string the frames are gotten from.
		/// </summary>
		[JsonProperty("frames_string")]
		public string FramesString { get; private set; }
		/// <summary>
		/// The frames of the post, for when it's a gif.
		/// </summary>
		[JsonProperty("frames")]
		public IList<YandereFrame> Frames { get; private set; }
		/// <summary>
		/// Whether or not notes can be made.
		/// </summary>
		[JsonProperty("is_note_locked")]
		public bool IsNoteLocked { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when the post was last noted.
		/// </summary>
		[JsonProperty("last_noted_at")]
		public int LastNotedAtTimestamp { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when the post was last commented on.
		/// </summary>
		[JsonProperty("last_commented_at")]
		public int LastCommentedAtTimestamp { get; private set; }

		[JsonProperty("width")]
		private int _Width = -1;
		[JsonProperty("height")]
		private int _Height = -1;
		[JsonProperty("tags")]
		private string _Tags = null;
	}
}