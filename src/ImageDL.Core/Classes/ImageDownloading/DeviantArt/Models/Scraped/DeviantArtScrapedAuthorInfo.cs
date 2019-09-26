using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtScrapedAuthorInfo
	{
		/// <summary>
		/// Any attributes associated with the user.
		/// </summary>
		[JsonProperty("attributes")]
		public long Attributes { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("symbol")]
		public string Symbol { get; private set; }

		/// <summary>
		/// The link to their profile picture.
		/// </summary>
		[JsonProperty("usericon")]
		public string UserIcon { get; private set; }

		/// <summary>
		/// The id of the user. This is not their UUID.
		/// </summary>
		[JsonProperty("userid")]
		public int UserId { get; private set; }

		/// <summary>
		/// Their name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// The guid of the user.
		/// </summary>
		[JsonProperty("uuid")]
		public string UUID { get; private set; }

		/// <summary>
		/// Returns the username and user id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Username} ({UserId})";
	}
}