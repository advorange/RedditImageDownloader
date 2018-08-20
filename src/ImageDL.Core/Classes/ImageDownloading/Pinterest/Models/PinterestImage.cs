using System;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// An image from Pinterest.
	/// </summary>
	public struct PinterestImage : ISize
	{
		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// Returns the url, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Url} ({Width}x{Height})";
		}
	}
}