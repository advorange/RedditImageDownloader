using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Filter color to apply to an image.
	/// </summary>
	public struct VscoPreset
	{
		/// <summary>
		/// The color applied with this preset.
		/// </summary>
		[JsonProperty("color")]
		public string Color { get; private set; }
		/// <summary>
		/// The key for this preset.
		/// </summary>
		[JsonProperty("key")]
		public string Key { get; private set; }
		/// <summary>
		/// Name representing the preset. Generally is just the key but in all capitals.
		/// </summary>
		[JsonProperty("short_name")]
		public string ShortName { get; private set; }
	}
}