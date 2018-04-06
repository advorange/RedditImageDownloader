using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtScrapedAuthorInfo
	{
		/// <summary>
		/// The id of the user. This is not their UUID.
		/// </summary>
		[JsonProperty("userid")]
		public readonly int UserId;
		/// <summary>
		/// Any attributes associated with the user.
		/// </summary>
		[JsonProperty("attributes")]
		public readonly long Attributes;
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("symbol")]
		public readonly string Symbol;
		/// <summary>
		/// Their name.
		/// </summary>
		[JsonProperty("username")]
		public readonly string Username;
		/// <summary>
		/// The link to their profile picture.
		/// </summary>
		[JsonProperty("usericon")]
		public readonly string UserIcon;
		/// <summary>
		/// The guid of the user.
		/// </summary>
		[JsonProperty("uuid")]
		public readonly string UUID;

		/// <summary>
		/// Returns the username and user id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Username} ({UserId})";
		}
	}
}