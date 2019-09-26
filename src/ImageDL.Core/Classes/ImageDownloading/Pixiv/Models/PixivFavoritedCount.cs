using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// The amount of people who have favorited.
	/// </summary>
	public sealed class PixivFavoritedCount
	{
		/// <summary>
		/// Count of private favorites.
		/// </summary>
		[JsonProperty("private")]
		public int Private { get; private set; }

		/// <summary>
		/// Count of public favorites.
		/// </summary>
		[JsonProperty("public")]
		public int Public { get; private set; }

		/// <summary>
		/// Returns the total count of favorites.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => (Public + Private).ToString();
	}
}