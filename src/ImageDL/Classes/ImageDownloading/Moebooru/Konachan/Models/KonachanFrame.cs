#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan.Models
{
	/// <summary>
	/// Json model for a frame, whatever that is.
	/// </summary>
	public struct KonachanFrame
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
