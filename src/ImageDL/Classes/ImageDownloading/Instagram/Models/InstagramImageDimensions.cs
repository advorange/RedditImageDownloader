using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the dimensions of the image.
	/// </summary>
	public struct InstagramImageDimensions
	{
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }

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