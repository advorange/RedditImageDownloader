using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a thumbnail.
	/// </summary>
	public struct InstagramThumbnail
	{
		/// <summary>
		/// The source of the image.
		/// </summary>
		[JsonProperty("src")]
		public readonly string Source;
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("config_width")]
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("config_height")]
		public readonly int Height;

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}