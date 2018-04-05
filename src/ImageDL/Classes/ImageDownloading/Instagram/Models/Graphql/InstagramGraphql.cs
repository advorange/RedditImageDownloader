#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql
{
	/// <summary>
	/// Holds the main media.
	/// </summary>
	public sealed class InstagramGraphql
	{
		[JsonProperty("shortcode_media")]
		public readonly InstagramMediaNode ShortcodeMedia;

		/// <inheritdoc />
		public override string ToString()
		{
			return ShortcodeMedia.ToString();
		}
	}
}