using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for an image from Imgur.
	/// </summary>
	public class ImgurImage : ImgurThing
	{
		/// <summary>
		/// The type of post this is.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// Whether the image is a gif/gifv/mp4.
		/// </summary>
		[JsonProperty("animated")]
		public bool IsAnimated { get; private set; }
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
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("size")]
		public long FileSize { get; private set; }
		/// <summary>
		/// How much bandwidth in bytes the file has used in total.
		/// </summary>
		[JsonProperty("bandwidth")]
		public long Bandwidth { get; private set; }
		/// <summary>
		/// Whether the image is in the public gallery.
		/// </summary>
		[JsonProperty("in_gallery")]
		public bool IsInGallery { get; private set; }
		/// <summary>
		/// Whether the post has sound.
		/// </summary>
		[JsonProperty("has_sound")]
		public bool HasSound { get; private set; }

		/// <inheritdoc />
		public override Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(new ImageResponse(FailureReason.Success, null, new[] { Mp4Link ?? PostUrl }));
		}
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