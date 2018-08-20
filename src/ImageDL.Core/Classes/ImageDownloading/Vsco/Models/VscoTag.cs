using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Tags on an image.
	/// </summary>
	public struct VscoTag
	{
		/// <summary>
		/// The name of the tag.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
		/// <summary>
		/// The value of the tag.
		/// </summary>
		[JsonProperty("slug")]
		public string Slug { get; private set; }
	}
}