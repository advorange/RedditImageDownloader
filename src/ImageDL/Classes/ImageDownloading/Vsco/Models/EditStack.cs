#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// The key of the preset.
	/// </summary>
	public struct EditStack
	{
		[JsonProperty("key")]
		public readonly string Key;
	}
}