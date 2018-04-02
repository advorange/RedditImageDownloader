#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the dimensions of the image.
	/// </summary>
	public struct Dimensions
	{
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}
