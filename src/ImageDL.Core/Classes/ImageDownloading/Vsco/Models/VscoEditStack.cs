using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// The key of the preset.
	/// </summary>
	public struct VscoEditStack
	{
		/// <summary>
		/// The key of the preset.
		/// </summary>
		[JsonProperty("key")]
		public string Key { get; private set; }
	}
}