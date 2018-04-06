using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct DeviantArtApiThumbnail
	{
		/// <summary>
		/// Whether or not the thumbnail is transparent.
		/// </summary>
		[JsonProperty("transparency")]
		public readonly bool IsTransparent;
		/// <summary>
		/// The thumbnail's width.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The thumbnail's height.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The size of the thumbnail in bytes.
		/// </summary>
		[JsonProperty("filesize")]
		public readonly long FileSize;
		/// <summary>
		/// The direct link to the thumbnail.
		/// </summary>
		[JsonProperty("src")]
		public readonly string Source;

		/// <summary>
		/// Returns with width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}