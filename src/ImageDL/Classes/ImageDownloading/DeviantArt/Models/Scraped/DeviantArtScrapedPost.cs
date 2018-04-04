#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via scraping a gallery.
	/// </summary>
	public sealed class DeviantArtScrapedPost
	{
		[JsonProperty("mature")]
		public readonly bool IsMature;
		[JsonProperty("faved")]
		public readonly bool IsFavorited;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("row")]
		public readonly int Row;
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("src", Required = Required.Always)]
		public readonly string Source;
		[JsonProperty("type")]
		public readonly string Type;
		[JsonProperty("alt")]
		public readonly string AltText;
		[JsonProperty("author")]
		public readonly DeviantArtScrapedAuthorInfo Author;
		[JsonProperty("sizing")]
		public readonly List<DeviantArtScrapedThumbnail> Sizes;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}
