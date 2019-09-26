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
	public sealed class DeviantArtScrapedPost : IPost, ISize
	{
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

		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => new DateTime(1970, 1, 1);

		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// If you have favorited the post. This will always be false because we're not signed in.
		/// </summary>
		[JsonProperty("faved")]
		public bool IsFavorited { get; private set; }

		/// <summary>
		/// Whether the post requires a user to agree to view mature content.
		/// </summary>
		[JsonProperty("mature")]
		public bool IsMature { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.fav.me/{Id}");

		/// <summary>
		/// The row the image is in in the gallery.
		/// </summary>
		[JsonProperty("row")]
		public int Row { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public int Score => -1;

		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonProperty("src", Required = Required.Always)]
		public Uri Source { get; private set; }

		/// <summary>
		/// The thumbnails of the image.
		/// </summary>
		[JsonProperty("sizing")]
		public IList<DeviantArtScrapedThumbnail> Thumbnails { get; private set; }

		/// <summary>
		/// The type of post this is, e.g. journal, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromUrl(Source));

		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Id} ({Width}x{Height})";
	}
}