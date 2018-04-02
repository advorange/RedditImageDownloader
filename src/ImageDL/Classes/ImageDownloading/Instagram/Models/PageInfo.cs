#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the pagination information.
	/// </summary>
	public struct PageInfo
	{
		[JsonProperty("has_next_page")]
		public readonly bool HasNextPage;
		[JsonProperty("end_cursor")]
		public readonly string EndCursor;

		/// <inheritdoc />
		public override string ToString()
		{
			return EndCursor;
		}
	}
}
