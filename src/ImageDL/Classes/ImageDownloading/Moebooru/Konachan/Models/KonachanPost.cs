using System;
using System.Collections.Generic;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan.Models
{
	/// <summary>
	/// Json model for a Konachan post.
	/// </summary>
	public sealed class KonachanPost : MoebooruPost
	{
		#region Json
		/// <summary>
		/// The id of whoever created the image.
		/// </summary>
		[JsonProperty("creator_id")]
		public readonly int CreatorId;
		/// <summary>
		/// The uploader of the image.
		/// </summary>
		[JsonProperty("author")]
		public readonly string Author;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("change")]
		public readonly int Change;
		/// <summary>
		/// If the post is listed.
		/// </summary>
		[JsonProperty("is_shown_in_index")]
		public readonly bool IsShownInIndex;
		/// <summary>
		/// The url to the 300x300 preview.
		/// </summary>
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
		/// <summary>
		/// The width of the preview scaled to 150px.
		/// </summary>
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		/// <summary>
		/// The height of the preview scaled to 150px.
		/// </summary>
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		/// <summary>
		/// The width of the preview.
		/// </summary>
		[JsonProperty("actual_preview_width")]
		public readonly int ActualPreviewWidth;
		/// <summary>
		/// The height of the preview.
		/// </summary>
		[JsonProperty("actual_preview_height")]
		public readonly int ActualPreviewHeight;
		/// <summary>
		/// Same as <see cref="JpgUrl"/> unless wider than 1500px.
		/// </summary>
		[JsonProperty("sample_url")]
		public readonly string SampleUrl;
		/// <summary>
		/// The width of the sample url.
		/// </summary>
		[JsonProperty("sample_width")]
		public readonly int SampleWidth;
		/// <summary>
		/// The height of the sample url.
		/// </summary>
		[JsonProperty("sample_height")]
		public readonly int SampleHeight;
		/// <summary>
		/// 0 if sample url is the same as jpg url.
		/// </summary>
		[JsonProperty("sample_file_size")]
		public readonly long SampleFileSize;
		/// <summary>
		/// The url if the image is a jpg. Will be scaled down to 3500px if wider.
		/// </summary>
		[JsonProperty("jpeg_url")]
		public readonly string JpgUrl;
		/// <summary>
		/// Width of the jpg url file.
		/// </summary>
		[JsonProperty("jpeg_width")]
		public readonly int JpgWidth;
		/// <summary>
		/// Height of the jpg url file.
		/// </summary>
		[JsonProperty("jpeg_height")]
		public readonly int JpgHeight;
		/// <summary>
		/// 0 if jpg url is the same as file url.
		/// </summary>
		[JsonProperty("jpeg_file_size")]
		public readonly long JpgFileSize;
		/// <summary>
		/// The status of the image, e.g. active, etc.
		/// </summary>
		[JsonProperty("status")]
		public readonly string Status;
		/// <summary>
		/// If the post has been held by a moderator.
		/// </summary>
		[JsonProperty("is_held")]
		public readonly bool IsHeld;
		/// <summary>
		/// The string the pending frames are gotten from.
		/// </summary>
		[JsonProperty("frames_pending_string")]
		public readonly string FramesPendingString;
		/// <summary>
		/// The pending frames of the post, for when it's a gif.
		/// </summary>
		[JsonProperty("frames_pending")]
		public readonly List<KonachanFrame> FramesPending;
		/// <summary>
		/// The string the frames are gotten from.
		/// </summary>
		[JsonProperty("frames_string")]
		public readonly string FramesString;
		/// <summary>
		/// The frames of the post, for when it's a gif.
		/// </summary>
		[JsonProperty("frames")]
		public readonly List<KonachanFrame> Frames;
		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("file_size")]
		public readonly long FileSize;
		/// <summary>
		/// The unix timestamp in seconds of when the post was created.
		/// </summary>
		[JsonProperty("created_at")]
		public readonly int CreatedAtTimestamp;
		[JsonProperty("width")]
		private readonly int _Width = -1;
		[JsonProperty("height")]
		private readonly int _Height = -1;
		[JsonProperty("tags")]
		private readonly string _Tags = null;
		#endregion

		/// <inheritdoc />
		public override Uri BaseUrl => new Uri("https://www.konachan.com");
		/// <inheritdoc />
		public override Uri PostUrl => new Uri($"{BaseUrl}/post/show/{Id}");
		/// <inheritdoc />
		public override DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <inheritdoc />
		public override int Width => _Width;
		/// <inheritdoc />
		public override int Height => _Height;
		/// <inheritdoc />
		public override string Tags => _Tags;
	}
}