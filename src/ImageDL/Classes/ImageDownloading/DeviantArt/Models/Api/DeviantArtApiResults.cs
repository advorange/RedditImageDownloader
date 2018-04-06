using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Json model for searching for DeviantArt posts through the API.
	/// </summary>
	public sealed class DeviantArtApiResults
	{
		/// <summary>
		/// Whether or not there are more posts.
		/// </summary>
		[JsonProperty("has_more")]
		public readonly bool HasMore;
		/// <summary>
		/// The next offset to start at.
		/// </summary>
		[JsonProperty("next_offset")]
		public readonly int? NextOffset;
		/// <summary>
		/// How many posts the user has submitted in total.
		/// </summary>
		[JsonProperty("estimated_total")]
		public readonly int EstimatedTotal;
		/// <summary>
		/// The posts gathered this query.
		/// </summary>
		[JsonProperty("results")]
		public readonly List<DeviantArtApiPost> Results;
	}
}