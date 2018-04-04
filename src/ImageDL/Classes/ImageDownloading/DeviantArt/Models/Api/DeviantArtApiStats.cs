#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about comments/favorites
	/// </summary>
	public struct DeviantArtApiStats
	{
		[JsonProperty("comments")]
		public readonly int Comments;
		[JsonProperty("favourites")]
		public readonly int Favorites;
	}
}
