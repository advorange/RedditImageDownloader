using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OAuth
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via the API.
	/// </summary>
	public sealed class DeviantArtOAuthPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => Content.Source.ToString().Split('-', '/').Last().Split('.')[0];
		/// <inheritdoc />
		[JsonProperty("url")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Stats.Favorites;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(PublishedTimestamp)).ToUniversalTime();
		/// <summary>
		/// If you have favorited the post. This will always be false because we're not signed in.
		/// </summary>
		[JsonProperty("is_favourited")]
		public bool IsFavorited { get; private set; }
		/// <summary>
		/// Whether the post is deleted or not.
		/// </summary>
		[JsonProperty("is_deleted")]
		public bool IsDeleted { get; private set; }
		/// <summary>
		/// Whether the post requires a user to agree to view mature content.
		/// </summary>
		[JsonProperty("is_mature")]
		public bool IsMature { get; private set; }
		/// <summary>
		/// Whether this post's image is downloadable.
		/// </summary>
		[JsonProperty("is_downloadable")]
		public bool IsDownloadable { get; private set; }
		/// <summary>
		/// Whether this post allows people to comment on it.
		/// </summary>
		[JsonProperty("allows_comments")]
		public bool AllowsComments { get; private set; }
		/// <summary>
		/// The size of the file.
		/// </summary>
		[JsonProperty("download_filesize")]
		public long DownloadFileSize { get; private set; }
		/// <summary>
		/// The guid of the post.
		/// </summary>
		[JsonProperty("deviationid")]
		public string DeviationUUID { get; private set; }
		/// <summary>
		/// The guid of the image?
		/// </summary>
		[JsonProperty("printid")]
		public string PrintUUID { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The category this post is in.
		/// </summary>
		[JsonProperty("category")]
		public string Category { get; private set; }
		/// <summary>
		/// The path to the category the post is in.
		/// </summary>
		[JsonProperty("category_path")]
		public string CategoryPath { get; private set; }
		/// <summary>
		/// Information about the author.
		/// </summary>
		[JsonProperty("author")]
		public DeviantArtOAuthAuthorInfo Author { get; private set; }
		/// <summary>
		/// Information about the post itself.
		/// </summary>
		[JsonProperty("stats")]
		public DeviantArtOAuthStats Stats { get; private set; }
		/// <summary>
		/// A small preview of the content.
		/// </summary>
		[JsonProperty("preview")]
		public DeviantArtOAuthThumbnail Preview { get; private set; }
		/// <summary>
		/// The main picture.
		/// </summary>
		[JsonProperty("content")]
		public DeviantArtOAuthThumbnail Content { get; private set; }
		/// <summary>
		/// The thumbnails of the post.
		/// </summary>
		[JsonProperty("thumbs")]
		public IList<DeviantArtOAuthThumbnail> Thumbnails { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when the post was published.
		/// </summary>
		[JsonProperty("published_time")]
		public long PublishedTimestamp { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(Content.Source));
		}
		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({Content.Width}x{Content.Height})";
		}
	}
}