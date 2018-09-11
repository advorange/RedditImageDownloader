using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information on an embed in artstation.
	/// </summary>
	public struct ArtstationOEmbed : ISize
	{
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// Where the embed comes from.
		/// </summary>
		[JsonProperty("provider_name")]
		public string ProviderName { get; private set; }

		/// <summary>
		/// Returns the provider name, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{ProviderName} ({Width}x{Height})";
	}
}
