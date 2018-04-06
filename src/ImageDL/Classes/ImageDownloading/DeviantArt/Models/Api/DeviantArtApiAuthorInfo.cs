using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtApiAuthorInfo
	{
		/// <summary>
		/// The guid of the user.
		/// </summary>
		[JsonProperty("userid")]
		public readonly string UUID;
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public readonly string Username;
		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("usericon")]
		public readonly string UserIcon;
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("type")]
		public readonly string Type;

		/// <summary>
		/// Returns the user's name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Username;
		}
	}
}