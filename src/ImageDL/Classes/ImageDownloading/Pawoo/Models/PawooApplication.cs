using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Information about how the post was uploaded.
	/// </summary>
	public struct PawooApplication
	{
		/// <summary>
		/// The type of application, e.g. web, etc.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The website this is from.
		/// </summary>
		[JsonProperty("website")]
		public string Website { get; private set; }
	}
}