using System;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Lofter.Models
{
	/// <summary>
	/// Scraped Html about an image from Lofter.
	/// </summary>
	public sealed class LofterImage : ISize
	{
		/// <summary>
		/// Url leading to the original image.
		/// </summary>
		[JsonProperty("full_image_url")]
		public Uri FullImageUrl { get; private set; }

		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// Url leading to a resized image.
		/// </summary>
		[JsonProperty("resized_image_url")]
		public Uri ResizedImageUrl { get; private set; }

		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="LofterImage"/>.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="url"></param>
		internal LofterImage(int width, int height, string url)
		{
			Width = width;
			Height = height;
			FullImageUrl = new Uri(url.Split('?')[0]);
			ResizedImageUrl = new Uri(url);
		}

		/// <summary>
		/// Returns the full image url, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{FullImageUrl} ({Width}x{Height})";
	}
}