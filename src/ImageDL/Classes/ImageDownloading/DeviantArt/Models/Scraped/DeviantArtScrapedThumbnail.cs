#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct DeviantArtScrapedThumbnail
	{
		[JsonProperty("transparent")]
		public readonly bool IsTransparent;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("src")]
		public readonly string Source;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}
