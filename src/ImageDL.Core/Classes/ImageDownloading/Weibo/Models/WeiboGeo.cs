using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about the size of the image.
	/// </summary>
	public struct WeiboGeo : ISize
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
		/// Whether the post is cropped.
		/// </summary>
		[JsonProperty("croped")]
		public bool Cropped { get; private set; }

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"({Width}x{Height})";
	}
}