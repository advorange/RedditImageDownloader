#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via scraping a gallery.
	/// </summary>
	public sealed class DeviantArtScrappedPost
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
		public readonly ScrapedAuthorInfo Author;
		[JsonProperty("sizing")]
		public readonly List<ScrapedThumbnail> Sizes;

		public override string ToString()
		{
			return Id.ToString();
		}
	}

	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct ScrapedThumbnail
	{
		[JsonProperty("transparent")]
		public readonly bool IsTransparent;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("src")]
		public readonly string Source;
	}

	/// <summary>
	/// Holds information about the author of the image.
	/// </summary>
	public struct ScrapedAuthorInfo
	{
		[JsonProperty("userid")]
		public readonly int UserId;
		[JsonProperty("attributes")]
		public readonly long Attributes;
		[JsonProperty("symbol")]
		public readonly string Symbol;
		[JsonProperty("username")]
		public readonly string Username;
		[JsonProperty("usericon")]
		public readonly string UserIcon;
		[JsonProperty("uuid")]
		public readonly string UUID;
	}
}