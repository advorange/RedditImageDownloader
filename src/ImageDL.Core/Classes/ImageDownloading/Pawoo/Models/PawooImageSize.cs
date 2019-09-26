using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// The size of an image.
	/// </summary>
	public struct PawooImageSize : ISize
	{
		/// <summary>
		/// The aspect ratio.
		/// </summary>
		[JsonProperty("aspect")]
		public double Aspect { get; private set; }

		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// The width x the height.
		/// </summary>
		[JsonProperty("size")]
		public string Size { get; private set; }

		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }

		/// <summary>
		/// Returns the width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Width}x{Height}";
	}
}