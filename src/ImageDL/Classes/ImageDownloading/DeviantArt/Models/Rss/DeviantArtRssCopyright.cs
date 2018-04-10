using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about the copyright on a post.
	/// </summary>
	public struct DeviantArtRssCopyright
	{
		/// <summary>
		/// The url of whoever posted this.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; private set; }
		/// <summary>
		/// The copyright text.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
	}
}