#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct DeviantArtApiAuthorInfo
	{
		[JsonProperty("userid")]
		public readonly string UUID;
		[JsonProperty("username")]
		public readonly string Username;
		[JsonProperty("usericon")]
		public readonly string UserIcon;
		[JsonProperty("type")]
		public readonly string Type;

		/// <inheritdoc />
		public override string ToString()
		{
			return Username;
		}
	}
}
