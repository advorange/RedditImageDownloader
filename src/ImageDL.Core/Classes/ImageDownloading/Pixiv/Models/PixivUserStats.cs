using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// The stats of the user.
	/// </summary>
	public sealed class PixivUserStats
	{
		/// <summary>
		/// How many favorites the user has.
		/// </summary>
		[JsonProperty("favorites")]
		public int Favorites { get; private set; }

		/// <summary>
		/// How many people the user is following.
		/// </summary>
		[JsonProperty("following")]
		public int Following { get; private set; }

		/// <summary>
		/// How many friends the user has.
		/// </summary>
		[JsonProperty("friends")]
		public int Friends { get; private set; }

		/// <summary>
		/// How many posts the user has made.
		/// </summary>
		[JsonProperty("works")]
		public int Works { get; private set; }
	}
}