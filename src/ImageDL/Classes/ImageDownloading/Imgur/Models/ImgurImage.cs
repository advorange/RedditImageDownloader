using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for an image from Imgur.
	/// </summary>
	public class ImgurImage : ImgurThing
	{
		#region Json
		/// <summary>
		/// The type of post this is.
		/// </summary>
		[JsonProperty("type")]
		public readonly string Type;
		/// <summary>
		/// Whether the image is a gif/gifv/mp4.
		/// </summary>
		[JsonProperty("animated")]
		public readonly bool IsAnimated;
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("size")]
		public readonly long FileSize;
		/// <summary>
		/// How much bandwidth in bytes the file has used in total.
		/// </summary>
		[JsonProperty("bandwidth")]
		public readonly long Bandwidth;
		/// <summary>
		/// Whether the image is in the public gallery.
		/// </summary>
		[JsonProperty("in_gallery")]
		public readonly bool IsInGallery;
		/// <summary>
		/// Whether the post has sound.
		/// </summary>
		[JsonProperty("has_sound")]
		public readonly bool HasSound;
		#endregion

		/// <inheritdoc />
		public override IEnumerable<Uri> ContentUrls => new[] { new Uri(Mp4Link ?? PostUrl.ToString()) };

		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}