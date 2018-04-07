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
		public bool IsTransparent { get; private set; }
		/// <summary>
		/// The width of the thumbnail.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The height of the thumbnail.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The direct link to the thumbnail.
		/// </summary>
		[JsonProperty("src")]
		public string Source { get; private set; }

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