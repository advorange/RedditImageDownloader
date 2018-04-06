using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for a tumblr Photo.
	/// </summary>
	public class TumblrPhoto
	{
		/// <summary>
		/// The offset of an image if it was in a post with more than one image.
		/// </summary>
		[JsonProperty("offset")]
		public readonly string Offset;
		/// <summary>
		/// The caption of the image.
		/// </summary>
		[JsonProperty("caption")]
		public readonly string Caption;
		/// <summary>
		/// The image's width.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The image's height.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The link to the image.
		/// </summary>
		[JsonProperty("photo-url-1280")]
		private readonly string _PhotoUrl = null;

		/// <summary>
		/// Returns the full size image url.
		/// </summary>
		public Uri PhotoUrl => TumblrImageGatherer.GetFullSizeImage(new Uri(_PhotoUrl));
	}
}