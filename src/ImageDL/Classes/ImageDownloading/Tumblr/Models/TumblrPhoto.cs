using ImageDL.Interfaces;
using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for a tumblr Photo.
	/// </summary>
	public class TumblrPhoto : ISize
	{
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The offset of an image if it was in a post with more than one image.
		/// </summary>
		[JsonProperty("offset")]
		public string Offset { get; private set; }
		/// <summary>
		/// The caption of the image.
		/// </summary>
		[JsonProperty("caption")]
		public string Caption { get; private set; }
		/// <summary>
		/// The link to the image.
		/// </summary>
		[JsonProperty("photo-url-1280")]
		public Uri RegularImageUrl { get; private set; }
		/// <summary>
		/// The link to the raw image.
		/// </summary>
		[JsonIgnore]
		public Uri FullSizeImageUrl => TumblrPostDownloader.GetFullSizeTumblrImage(RegularImageUrl);

		/// <summary>
		/// Returns the url, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{FullSizeImageUrl} ({Width}x{Height})";
		}
	}
}