using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OEmbed
{
	/// <summary>
	/// Holds information about how people have interacted with this post.
	/// </summary>
	public struct DeviantArtOEmbedCommunity
	{
		/// <summary>
		/// Needless nested property for the statistics.
		/// </summary>
		[JsonProperty("statistics")]
		public DeviantArtOEmbedStatistics Statistics { get; private set; }
	}

	/// <summary>
	/// Needless wrapper class for the statistics.
	/// </summary>
	public struct DeviantArtOEmbedStatistics
	{
		/// <summary>
		/// Information about the statistics.
		/// </summary>
		[JsonProperty("_attributes")]
		public DeviantArtOEmbedStatisticsAttributes Attributes { get; private set; }
	}

	/// <summary>
	/// Information about the statistics.
	/// </summary>
	public struct DeviantArtOEmbedStatisticsAttributes
	{
		/// <summary>
		/// How many comments are on this image.
		/// </summary>
		[JsonProperty("comments")]
		public int Comments { get; private set; }

		/// <summary>
		/// How many people have downloaded this image.
		/// </summary>
		[JsonProperty("downloads")]
		public int Downloads { get; private set; }

		/// <summary>
		/// How many people have favorited this image.
		/// </summary>
		[JsonProperty("favorites")]
		public int Favorites { get; private set; }

		/// <summary>
		/// How many people have looked at this image.
		/// </summary>
		[JsonProperty("views")]
		public int Views { get; private set; }
	}
}