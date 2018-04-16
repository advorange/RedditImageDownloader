using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models
{
	/// <summary>
	/// Json model for the results of a Twitter page.
	/// </summary>
	public struct TwitterPage
	{
		/// <summary>
		/// Whether there are more items to be found.
		/// </summary>
		[JsonProperty("has_more_items")]
		public bool HasMore { get; private set; }
		/// <summary>
		/// The lowest value id of the tweets found.
		/// </summary>
		[JsonProperty("min_position")]
		public ulong MinPosition { get; private set; }
		/// <summary>
		/// The html of the items.
		/// </summary>
		[JsonProperty("items_html")]
		public string ItemsHtml { get; private set; }
	}
}
