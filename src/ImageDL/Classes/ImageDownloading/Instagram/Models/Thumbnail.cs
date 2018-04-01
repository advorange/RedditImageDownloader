#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a thumbnail.
	/// </summary>
	public struct Thumbnail
	{
		[JsonProperty("src")]
		public readonly string Source;
		[JsonProperty("config_width")]
		public readonly int Width;
		[JsonProperty("config_height")]
		public readonly int Height;
	}
}
