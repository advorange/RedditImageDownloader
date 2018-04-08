using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via scraping a gallery.
	/// </summary>
	public sealed class DeviantArtScrapedPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.fav.me/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => -1;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => new DateTime(1970, 1, 1);
		/// <summary>
		/// Whether the post requires a user to agree to view mature content.
		/// </summary>
		[JsonProperty("mature")]
		public bool IsMature { get; private set; }
		/// <summary>
		/// If you have favorited the post. This will always be false because we're not signed in.
		/// </summary>
		[JsonProperty("faved")]
		public bool IsFavorited { get; private set; }
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
		/// The row the image is in in the gallery.
		/// </summary>
		[JsonProperty("row")]
		public int Row { get; private set; }
		/// <summary>
		/// The type of post this is, e.g. journal, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// Other text associated with the image.
		/// </summary>
		[JsonProperty("alt")]
		public string AltText { get; private set; }
		/// <summary>
		/// Information about the author.
		/// </summary>
		[JsonProperty("author")]
		public DeviantArtScrapedAuthorInfo Author { get; private set; }
		/// <summary>
		/// The thumbnails of the image.
		/// </summary>
		[JsonProperty("sizing")]
		public List<DeviantArtScrapedThumbnail> Thumbnails { get; private set; }
		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonProperty("src", Required = Required.Always)]
		public Uri Source { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(Source));
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