using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds a GUID for DeviantArt.
	/// </summary>
	public struct DeviantArtRssGuid
	{
		/// <summary>
		/// Whether or not <see cref="Text"/> is a permalink.
		/// </summary>
		[JsonProperty("isPermaLink")]
		public string IsPermaLink { get; private set; }
		/// <summary>
		/// Can potentially be a link, or is other key.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
	}
}