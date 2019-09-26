using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OAuth
{
	/// <summary>
	/// Json model for searching for DeviantArt posts through the API.
	/// </summary>
	public struct DeviantArtOAuthResults
	{
		/// <summary>
		/// How many posts the user has submitted in total.
		/// </summary>
		[JsonProperty("estimated_total")]
		public int EstimatedTotal { get; private set; }

		/// <summary>
		/// Whether or not there are more posts.
		/// </summary>
		[JsonProperty("has_more")]
		public bool HasMore { get; private set; }

		/// <summary>
		/// The next offset to start at.
		/// </summary>
		[JsonProperty("next_offset")]
		public int? NextOffset { get; private set; }

		/// <summary>
		/// The posts gathered this query.
		/// </summary>
		[JsonProperty("results")]
		public IList<DeviantArtOAuthPost> Results { get; private set; }
	}
}