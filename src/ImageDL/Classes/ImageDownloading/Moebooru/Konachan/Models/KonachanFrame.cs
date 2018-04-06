using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan.Models
{
	/// <summary>
	/// Json model for a frame, whatever that is.
	/// </summary>
	public struct KonachanFrame
	{
		/// <summary>
		/// The left most part of the frame.
		/// </summary>
		[JsonProperty("source_left")]
		public readonly int SourceLeft;
		/// <summary>
		/// The top most part of the frame.
		/// </summary>
		[JsonProperty("source_top")]
		public readonly int SourceTop;
		/// <summary>
		/// The width of the frame.
		/// </summary>
		[JsonProperty("source_width")]
		public readonly int SourceWidth;
		/// <summary>
		/// The height of the frame.
		/// </summary>
		[JsonProperty("source_height")]
		public readonly int SourceHeight;
		/// <summary>
		/// The id of the post this belongs to.
		/// </summary>
		[JsonProperty("post_id")]
		public readonly int PostId;
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The preview width.
		/// </summary>
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		/// <summary>
		/// The preview height.
		/// </summary>
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		/// <summary>
		/// The url to the frame.
		/// </summary>
		[JsonProperty("url")]
		public readonly string Url;
		/// <summary>
		/// The url to the preview.
		/// </summary>
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
	}
}