#pragma warning disable 1591, 649
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan.Models
{
	/// <summary>
	/// Json model for a Konachan post.
	/// </summary>
	public sealed class KonachanPost : MoebooruPost
	{
		[JsonProperty("creator_id")]
		public readonly int CreatorId;
		[JsonProperty("author")]
		public readonly string Author;
		[JsonProperty("change")]
		public readonly int Change;
		[JsonProperty("is_shown_in_index")]
		public readonly bool IsShownInIndex;
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		[JsonProperty("actual_preview_width")]
		public readonly int ActualPreviewWidth;
		[JsonProperty("actual_preview_height")]
		public readonly int ActualPreviewHeight;
		[JsonProperty("sample_url")]
		public readonly string SampleUrl;
		[JsonProperty("sample_width")]
		public readonly int SampleWidth;
		[JsonProperty("sample_height")]
		public readonly int SampleHeight;
		[JsonProperty("sample_file_size")]
		public readonly long SampleFileSize;
		[JsonProperty("jpeg_url")]
		public readonly string JpgUrl;
		[JsonProperty("jpeg_width")]
		public readonly int JpgWidth;
		[JsonProperty("jpeg_height")]
		public readonly int JpgHeight;
		[JsonProperty("jpeg_file_size")]
		public readonly long JpgFileSize;
		[JsonProperty("status")]
		public readonly string Status;
		[JsonProperty("is_held")]
		public readonly bool IsHeld;
		[JsonProperty("frames_pending_string")]
		public readonly string FramesPendingString;
		[JsonProperty("frames_pending")]
		public readonly List<Frame> FramesPending;
		[JsonProperty("frames_string")]
		public readonly string FramesString;
		[JsonProperty("frames")]
		public readonly List<Frame> Frames;
		[JsonProperty("file_size")]
		public readonly long FileSize;
		[JsonProperty("width")]
		private readonly int _Width;
		[JsonProperty("height")]
		private readonly int _Height;
		[JsonProperty("created_at")]
		private readonly int _CreatedAt;
		[JsonProperty("tags")]
		private readonly string _Tags;

		[JsonIgnore]
		public override string BaseUrl => "https://www.konachan.com";
		[JsonIgnore]
		public override string PostUrl => $"{BaseUrl}/post/show/{Id}";
		[JsonIgnore]
		public override int Width => _Width;
		[JsonIgnore]
		public override int Height => _Height;
		[JsonIgnore]
		public override DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(_CreatedAt)).ToUniversalTime();
		[JsonIgnore]
		public override string Tags => _Tags;
	}

	/// <summary>
	/// Json model for a frame, whatever that is.
	/// </summary>
	public struct Frame
	{
		[JsonProperty("source_left")]
		public readonly int SourceLeft;
		[JsonProperty("source_top")]
		public readonly int SourceTop;
		[JsonProperty("source_width")]
		public readonly int SourceWidth;
		[JsonProperty("source_height")]
		public readonly int SourceHeight;
		[JsonProperty("post_id")]
		public readonly int PostId;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		[JsonProperty("url")]
		public readonly string Url;
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
	}
}