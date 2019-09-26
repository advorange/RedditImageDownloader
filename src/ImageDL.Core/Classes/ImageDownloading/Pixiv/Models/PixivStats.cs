using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// The stats of a post.
	/// </summary>
	public sealed class PixivStats
	{
		/// <summary>
		/// How many comments something has.
		/// </summary>
		[JsonProperty("commented_count")]
		public int CommentedCount { get; private set; }

		/// <summary>
		/// How many favorites something has.
		/// </summary>
		[JsonProperty("favorited_count")]
		public PixivFavoritedCount FavoritedCount { get; private set; }

		/// <summary>
		/// The score of the post.
		/// </summary>
		[JsonProperty("score")]
		public int Score { get; private set; }

		/// <summary>
		/// How many likes something has.
		/// </summary>
		[JsonProperty("scored_count")]
		public int ScoredCount { get; private set; }

		/// <summary>
		/// How many views something has.
		/// </summary>
		[JsonProperty("views_count")]
		public int ViewsCount { get; private set; }
	}
}