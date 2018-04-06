using System;
using System.Collections.Generic;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via scraping a gallery.
	/// </summary>
	public sealed class DeviantArtScrapedPost : IPost
	{
		#region Json
		/// <summary>
		/// Whether the post requires a user to agree to view mature content.
		/// </summary>
		[JsonProperty("mature")]
		public readonly bool IsMature;
		/// <summary>
		/// If you have favorited the post. This will always be false because we're not signed in.
		/// </summary>
		[JsonProperty("faved")]
		public readonly bool IsFavorited;
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
		/// The row the image is in in the gallery.
		/// </summary>
		[JsonProperty("row")]
		public readonly int Row;
		/// <summary>
		/// The type of post this is, e.g. journal, etc.
		/// </summary>
		[JsonProperty("type")]
		public readonly string Type;
		/// <summary>
		/// Other text associated with the image.
		/// </summary>
		[JsonProperty("alt")]
		public readonly string AltText;
		/// <summary>
		/// Information about the author.
		/// </summary>
		[JsonProperty("author")]
		public readonly DeviantArtScrapedAuthorInfo Author;
		/// <summary>
		/// The thumbnails of the image.
		/// </summary>
		[JsonProperty("sizing")]
		public readonly List<DeviantArtScrapedThumbnail> Thumbnails;
		/// <summary>
		/// The id of the image.
		/// </summary>
		[JsonProperty("id")]
		private readonly string _Id = null;
		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonProperty("src", Required = Required.Always)]
		private readonly string _Source = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri($"https://www.fav.me/{Id}");
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => new[] { new Uri(_Source) };
		/// <inheritdoc />
		public int Score => -1;
		/// <inheritdoc />
		public DateTime CreatedAt => new DateTime(1970, 1, 1);

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