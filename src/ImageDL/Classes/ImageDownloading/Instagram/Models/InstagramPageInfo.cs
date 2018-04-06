using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the pagination information.
	/// </summary>
	public struct InstagramPageInfo
	{
		/// <summary>
		/// Whether there's more to paginate.
		/// </summary>
		[JsonProperty("has_next_page")]
		public readonly bool HasNextPage;
		/// <summary>
		/// Where to start off the next pagination.
		/// </summary>
		[JsonProperty("end_cursor")]
		public readonly string EndCursor;

		/// <summary>
		/// Returns the end position.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return EndCursor;
		}
	}
}