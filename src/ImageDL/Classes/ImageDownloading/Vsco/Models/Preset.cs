#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Filter color to apply to an image.
	/// </summary>
	public struct Preset
	{
		[JsonProperty("color")]
		public readonly string Color;
		[JsonProperty("key")]
		public readonly string Key;
		[JsonProperty("short_name")]
		public readonly string ShortName;
	}
}
