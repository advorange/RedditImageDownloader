using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct DeviantArtScrapedThumbnail
	{
		/// <summary>
		/// Whether or not the thumbnail is transparent.
		/// </summary>
		[JsonProperty("transparent")]
		public readonly bool IsTransparent;
		/// <summary>
		/// The width of the thumbnail.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The height of the thumbnail.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The direct link to the thumbnail.
		/// </summary>
		[JsonProperty("src")]
		public readonly string Source;

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}