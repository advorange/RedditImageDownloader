using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Data for a Pinterest post.
	/// </summary>
	public class PinterestAggregatePostData
	{
		/// <summary>
		/// Random needless nesting.
		/// </summary>
		[JsonProperty("aggregated_stats")]
		public PinterestAggregateStats AggregatedStats { get; private set; }

		/// <summary>
		/// Returns <see cref="AggregatedStats"/> as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return AggregatedStats.ToString();
		}
	}
}