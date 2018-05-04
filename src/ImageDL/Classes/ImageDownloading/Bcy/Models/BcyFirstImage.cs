using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// The first image in a post.
	/// </summary>
	public struct BcyFirstImage : ISize
	{
		/// <summary>
		/// The url to the image.
		/// </summary>
		[JsonProperty("path")]
		public string Path { get; private set; }
		/// <inheritdoc />
		[JsonProperty("w")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("h")]
		public int Height { get; private set; }

		/// <summary>
		/// Returns the path, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Path} ({Width}x{Height})";
		}
	}
}