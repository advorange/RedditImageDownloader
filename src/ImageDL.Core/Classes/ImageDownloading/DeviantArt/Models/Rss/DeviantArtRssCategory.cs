using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about where a post is located.
	/// </summary>
	public struct DeviantArtRssCategory
	{
		/// <summary>
		/// The category's name.
		/// </summary>
		[JsonProperty("label")]
		public string Label { get; private set; }

		/// <summary>
		/// The category's path.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
	}
}