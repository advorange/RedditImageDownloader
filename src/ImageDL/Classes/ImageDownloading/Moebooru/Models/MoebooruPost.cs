using System;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Models
{
	/// <summary>
	/// Base Json model for a post from a Moebooru site.
	/// </summary>
	public abstract class MoebooruPost : IPost
	{
		/// <inhertitdoc />
		public abstract Uri PostUrl { get; }
		/// <inheritdoc />
		public abstract DateTime CreatedAt { get; }
		/// <summary>
		/// The base url of the -booru site. E.G.: https://danbooru.donmai.us or https://www.konachan.com
		/// </summary>
		public abstract Uri BaseUrl { get; }
		/// <summary>
		/// The width of the image.
		/// </summary>
		public abstract int Width { get; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		public abstract int Height { get; }
		/// <summary>
		/// The tags for this image.
		/// </summary>
		public abstract string Tags { get; }
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonProperty("score")]
		public int Score { get; private set; } = -1;
		/// <summary>
		/// The source of the image. Can be empty/null if no source is provided.
		/// </summary>
		[JsonProperty("source")]
		public string Source { get; private set; }
		/// <summary>
		/// The rating of the image. Safe, questionable, or explicit.
		/// </summary>
		[JsonProperty("rating")]
		public char Rating { get; private set; }
		/// <summary>
		/// The id of the parent post to this post if there is one.
		/// </summary>
		[JsonProperty("parent_id")]
		public int? ParentId { get; private set; }
		/// <summary>
		/// The hash of the image.
		/// </summary>
		[JsonProperty("md5")]
		public string MD5 { get; private set; }
		/// <summary>
		/// Whether or not the post has children.
		/// </summary>
		[JsonProperty("has_children")]
		public bool HasChildren { get; private set; }
		/// <summary>
		/// The location of the file for this post. May be missing http/https.
		/// </summary>
		[JsonProperty("file_url")]
		public string FileUrl { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			if (Uri.TryCreate(FileUrl, UriKind.Absolute, out var url) ||
				Uri.TryCreate($"{BaseUrl}{FileUrl}", UriKind.Absolute, out url))
			{
				return Task.FromResult(ImageResponse.FromUrl(url));
			}
			return Task.FromResult(ImageResponse.FromNotFound(PostUrl));
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