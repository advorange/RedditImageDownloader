#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the dimensions of the image.
	/// </summary>
	public struct Dimensions
	{
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("width")]
		public readonly int Width;
	}
}
