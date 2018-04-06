using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql
{
	/// <summary>
	/// Holds the main media.
	/// </summary>
	public sealed class InstagramGraphql
	{
		/// <summary>
		/// The information of a user's page.
		/// </summary>
		[JsonProperty("shortcode_media")]
		public readonly InstagramMediaNode ShortcodeMedia;

		/// <inheritdoc />
		public override string ToString()
		{
			return ShortcodeMedia.ToString();
		}
	}
}