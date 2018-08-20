using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Json model for the results of a Pixiv page.
	/// </summary>
	public sealed class PixivPage
	{
		/// <summary>
		/// Success or failure.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; private set; }
		/// <summary>
		/// The gotten posts.
		/// </summary>
		[JsonProperty("response")]
		public IList<PixivPost> Posts { get; private set; }
		/// <summary>
		/// How many posts were gotten.
		/// </summary>
		[JsonProperty("count")]
		public int Count { get; private set; }
		/// <summary>
		/// The current pagination information.
		/// </summary>
		[JsonProperty("pagination")]
		public PixivPagination Pagination { get; private set; }
	}
}