#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for an image from Imgur.
	/// </summary>
	public class ImgurImage : ImgurThing
	{
		[JsonProperty("type")]
		public readonly string Type;
		[JsonProperty("animated")]
		public readonly bool IsAnimated;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("size")]
		public readonly long FileSize;
		[JsonProperty("bandwidth")]
		public readonly long BandWidth;
		[JsonProperty("in_gallery")]
		public readonly bool IsInGallery;
		[JsonProperty("has_sound")]
		public readonly bool HasSound;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}
