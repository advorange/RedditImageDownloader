using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information on an embed in artstation.
	/// </summary>
	public struct ArtstationOEmbed
	{
		/// <summary>
		/// The width of the embed.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The height of the embed.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// Where the embed comes from.
		/// </summary>
		[JsonProperty("provider_name")]
		public string ProviderName { get; private set; }
	}
}
