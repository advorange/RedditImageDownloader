using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OAuth
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtOAuthAuthorInfo
	{
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("usericon")]
		public string UserIcon { get; private set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// The guid of the user.
		/// </summary>
		[JsonProperty("userid")]
		public string UUID { get; private set; }

		/// <summary>
		/// Returns the user's name.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Username;
	}
}