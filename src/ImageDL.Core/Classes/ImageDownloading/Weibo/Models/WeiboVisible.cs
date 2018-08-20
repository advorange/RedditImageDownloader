using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about the visibility of a post.
	/// </summary>
	public struct WeiboVisible
	{
		/// <summary>
		/// The type of visibility.
		/// </summary>
		[JsonProperty("type")]
		public int Type { get; private set; }
		/// <summary>
		/// What list this belongs to.
		/// </summary>
		[JsonProperty("list_id")]
		public int ListId { get; private set; }
	}
}