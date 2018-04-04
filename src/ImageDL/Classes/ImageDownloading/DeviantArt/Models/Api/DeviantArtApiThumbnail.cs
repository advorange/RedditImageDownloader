#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct DeviantArtApiThumbnail
	{
		[JsonProperty("transparency")]
		public readonly bool IsTransparent;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("filesize")]
		public readonly long FileSize;
		[JsonProperty("src")]
		public readonly string Source;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}
