using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Json model for a Konachan post.
	/// </summary>
	public class KonachanPost : BooruPost
	{
#pragma warning disable 1591
		[JsonProperty("tags")]
		public readonly string Tags;
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
		[JsonProperty("width")]
		public override int Width { get; }
		[JsonProperty("height")]
		public override int Height { get; }
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

		/// <summary>
		/// Json model for a frame, whatever that is.
		/// </summary>
		public class Frame
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
#pragma warning restore 1591
	}
}