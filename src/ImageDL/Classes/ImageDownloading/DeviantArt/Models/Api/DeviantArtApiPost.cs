#pragma warning disable 1591, 649
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via the API.
	/// </summary>
	public sealed class DeviantArtApiPost
	{
		[JsonProperty("is_favourited")]
		public readonly bool IsFavorited;
		[JsonProperty("is_deleted")]
		public readonly bool IsDeleted;
		[JsonProperty("is_mature")]
		public readonly bool IsMature;
		[JsonProperty("is_downloadable")]
		public readonly bool IsDownloadable;
		[JsonProperty("allows_comments")]
		public readonly bool AllowsComments;
		[JsonProperty("download_filesize")]
		public readonly long DownloadFileSize;
		[JsonProperty("deviationid")]
		public readonly string DeviationUUID;
		[JsonProperty("printid")]
		public readonly string PrintUUID;
		[JsonProperty("url")]
		public readonly string Url;
		[JsonProperty("title")]
		public readonly string Title;
		[JsonProperty("category")]
		public readonly string Category;
		[JsonProperty("category_path")]
		public readonly string CategoryPath;
		[JsonProperty("author")]
		public readonly DeviantArtApiAuthorInfo Author;
		[JsonProperty("stats")]
		public readonly DeviantArtApiStats Stats;
		[JsonProperty("preview")]
		public readonly DeviantArtApiThumbnail Preview;
		[JsonProperty("content")]
		public readonly DeviantArtApiThumbnail Content;
		[JsonProperty("thumbs")]
		public readonly List<DeviantArtApiThumbnail> Thumbnails;
		[JsonProperty("published_time")]
		private readonly long _PublishedTime;

		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(_PublishedTime)).ToUniversalTime();

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Url.Split('-').Last()} ({Content.Width}x{Content.Height})";
		}
	}
}
