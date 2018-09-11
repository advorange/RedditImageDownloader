using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds the dimensions of the image.
	/// </summary>
	public struct InstagramImageDimensions : ISize
	{
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Width}x{Height}";
	}
}