using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// The amount of people who have favorited.
	/// </summary>
	public sealed class PixivFavoritedCount
	{
		/// <summary>
		/// Count of public favorites.
		/// </summary>
		[JsonProperty("public")]
		public int Public { get; private set; }
		/// <summary>
		/// Count of private favorites.
		/// </summary>
		[JsonProperty("private")]
		public int Private { get; private set; }
	}
}