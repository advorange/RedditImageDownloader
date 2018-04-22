using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Pagination info for Pixiv.
	/// </summary>
	public sealed class PixivPagination
	{
		/// <summary>
		/// The previous page.
		/// </summary>
		[JsonProperty("previous")]
		public int? Previous { get; private set; }
		/// <summary>
		/// The next page.
		/// </summary>
		[JsonProperty("next")]
		public int? Next { get; private set; }
		/// <summary>
		/// The current page.
		/// </summary>
		[JsonProperty("current")]
		public int Current { get; private set; }
		/// <summary>
		/// How many being viewed.
		/// </summary>
		[JsonProperty("per_page")]
		public int PerPage { get; private set; }
		/// <summary>
		/// How many posts the person has.
		/// </summary>
		[JsonProperty("total")]
		public int Total { get; private set; }
		/// <summary>
		/// How many pages exist.
		/// </summary>
		[JsonProperty("pages")]
		public int Pages { get; private set; }
	}
}