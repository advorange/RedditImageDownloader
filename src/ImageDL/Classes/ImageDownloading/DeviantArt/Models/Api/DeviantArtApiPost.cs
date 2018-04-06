using System;
using System.Collections.Generic;
using System.Linq;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via the API.
	/// </summary>
	public sealed class DeviantArtApiPost : IPost
	{
		#region Json
		/// <summary>
		/// If you have favorited the post. This will always be false because we're not signed in.
		/// </summary>
		[JsonProperty("is_favourited")]
		public readonly bool IsFavorited;
		/// <summary>
		/// Whether the post is deleted or not.
		/// </summary>
		[JsonProperty("is_deleted")]
		public readonly bool IsDeleted;
		/// <summary>
		/// Whether the post requires a user to agree to view mature content.
		/// </summary>
		[JsonProperty("is_mature")]
		public readonly bool IsMature;
		/// <summary>
		/// Whether this post's image is downloadable.
		/// </summary>
		[JsonProperty("is_downloadable")]
		public readonly bool IsDownloadable;
		/// <summary>
		/// Whether this post allows people to comment on it.
		/// </summary>
		[JsonProperty("allows_comments")]
		public readonly bool AllowsComments;
		/// <summary>
		/// The size of the file.
		/// </summary>
		[JsonProperty("download_filesize")]
		public readonly long DownloadFileSize;
		/// <summary>
		/// The guid of the post.
		/// </summary>
		[JsonProperty("deviationid")]
		public readonly string DeviationUUID;
		/// <summary>
		/// The guid of the image?
		/// </summary>
		[JsonProperty("printid")]
		public readonly string PrintUUID;
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public readonly string Title;
		/// <summary>
		/// The category this post is in.
		/// </summary>
		[JsonProperty("category")]
		public readonly string Category;
		/// <summary>
		/// The path to the category the post is in.
		/// </summary>
		[JsonProperty("category_path")]
		public readonly string CategoryPath;
		/// <summary>
		/// Information about the author.
		/// </summary>
		[JsonProperty("author")]
		public readonly DeviantArtApiAuthorInfo Author;
		/// <summary>
		/// Information about the post itself.
		/// </summary>
		[JsonProperty("stats")]
		public readonly DeviantArtApiStats Stats;
		/// <summary>
		/// A small preview of the content.
		/// </summary>
		[JsonProperty("preview")]
		public readonly DeviantArtApiThumbnail Preview;
		/// <summary>
		/// The main picture.
		/// </summary>
		[JsonProperty("content")]
		public readonly DeviantArtApiThumbnail Content;
		/// <summary>
		/// The thumbnails of the post.
		/// </summary>
		[JsonProperty("thumbs")]
		public readonly List<DeviantArtApiThumbnail> Thumbnails;
		/// <summary>
		/// The unix timestamp in seconds of when the post was published.
		/// </summary>
		[JsonProperty("published_time")]
		public readonly long PublishedTimestamp;
		/// <summary>
		/// The link to the post.
		/// </summary>
		[JsonProperty("url")]
		private readonly string _Url = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Url.Split('-').Last();
		/// <inheritdoc />
		public Uri PostUrl => new Uri(_Url);
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => new[] { new Uri(Content.Source) };
		/// <inheritdoc />
		public int Score => Stats.Favorites;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(PublishedTimestamp)).ToUniversalTime();

		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{_Url.Split('-').Last()} ({Content.Width}x{Content.Height})";
		}
	}
}