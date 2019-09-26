using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about who posted an image.
	/// </summary>
	public struct DeviantArtRssCredit
	{
		/// <summary>
		/// The person's role, e.g. author.
		/// </summary>
		[JsonProperty("role")]
		public string Role { get; private set; }

		/// <summary>
		/// Not sure, but is usually 'urn:ebu'.
		/// </summary>
		[JsonProperty("scheme")]
		public string Scheme { get; private set; }

		/// <summary>
		/// The name of the person.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
	}
}