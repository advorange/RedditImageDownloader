#pragma warning disable 1591, 649
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Json model for searching for DeviantArt posts through the API.
	/// </summary>
	public sealed class DeviantArtApiResults
	{
		[JsonProperty("has_more")]
		public readonly bool HasMore;
		[JsonProperty("next_offset")]
		public readonly int? NextOffset;
		[JsonProperty("estimated_total")]
		public readonly int EstimatedTotal;
		[JsonProperty("results")]
		public readonly List<DeviantArtApiPost> Results;
	}
}
