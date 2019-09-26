using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds the description of a post.
	/// </summary>
	public struct DeviantArtRssDescription
	{
		/// <summary>
		/// The description's text.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// The type of content. Is either html or plain.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
	}
}