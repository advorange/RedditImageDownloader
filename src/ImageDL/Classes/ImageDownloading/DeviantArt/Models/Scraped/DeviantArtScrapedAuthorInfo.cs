#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtScrapedAuthorInfo
	{
		[JsonProperty("userid")]
		public readonly int UserId;
		[JsonProperty("attributes")]
		public readonly long Attributes;
		[JsonProperty("symbol")]
		public readonly string Symbol;
		[JsonProperty("username")]
		public readonly string Username;
		[JsonProperty("usericon")]
		public readonly string UserIcon;
		[JsonProperty("uuid")]
		public readonly string UUID;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Username} ({UserId})";
		}
	}
}
