using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information.
	/// </summary>
	public sealed class InstagramData
	{
		/// <summary>
		/// Information about a user.
		/// </summary>
		[JsonProperty("user")]
		public readonly InstagramUserInfo User;
	}
}