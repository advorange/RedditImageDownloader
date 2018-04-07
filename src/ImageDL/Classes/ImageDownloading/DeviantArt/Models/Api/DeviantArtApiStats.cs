using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about comments/favorites
	/// </summary>
	public struct DeviantArtApiStats
	{
		/// <summary>
		/// How many comments this post has.
		/// </summary>
		[JsonProperty("comments")]
		public int Comments { get; private set; }
		/// <summary>
		/// How many favorites this post has.
		/// </summary>
		[JsonProperty("favourites")]
		public int Favorites { get; private set; }
	}
}