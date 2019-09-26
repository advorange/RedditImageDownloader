using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a thumbnail.
	/// </summary>
	public struct InstagramThumbnail
	{
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("config_height")]
		public int Height { get; private set; }

		/// <summary>
		/// The source of the image.
		/// </summary>
		[JsonProperty("src")]
		public string Source { get; private set; }

		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("config_width")]
		public int Width { get; private set; }

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Width}x{Height}";
	}
}