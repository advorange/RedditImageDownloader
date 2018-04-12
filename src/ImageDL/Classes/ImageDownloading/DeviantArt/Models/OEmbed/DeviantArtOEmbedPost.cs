using System;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OEmbed
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via OEmbed.
	/// </summary>
	public sealed class DeviantArtOEmbedPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => ImageUrl.ToString().Split('-', '/').Last().Split('.').First();
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.fav.me/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Community.Statistics.Attributes.Favorites;
		/// <inheritdoc />
		[JsonProperty("pubdate")]
		public DateTime CreatedAt { get; private set; }
		/// <summary>
		/// The OEmbed version.
		/// </summary>
		[JsonProperty("version")]
		public string Version { get; private set; }
		/// <summary>
		/// The type of post, e.g. photo.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The category of the post.
		/// </summary>
		[JsonProperty("category")]
		public string Category { get; private set; }
		/// <summary>
		/// The link to the post's image.
		/// </summary>
		[JsonProperty("url")]
		public Uri ImageUrl { get; private set; }
		/// <summary>
		/// The name of whoever posted the image.
		/// </summary>
		[JsonProperty("author_name")]
		public string AuthorName { get; private set; }
		/// <summary>
		/// The url to the poster's account.
		/// </summary>
		[JsonProperty("author_url")]
		public Uri AuthorUrl { get; private set; }
		/// <summary>
		/// Says DeviantArt's name.
		/// </summary>
		[JsonProperty("provider_name")]
		public string ProviderName { get; private set; }
		/// <summary>
		/// Link to DeviantArt.
		/// </summary>
		[JsonProperty("provider_url")]
		public Uri ProviderUrl { get; private set; }
		/// <summary>
		/// Whether the picture is adult or safe. Similar to <see cref="Rating"/>.
		/// </summary>
		[JsonProperty("safety")]
		public string Safety { get; private set; }
		/// <summary>
		/// Information about comments, views, etc.
		/// </summary>
		[JsonProperty("community")]
		public DeviantArtOEmbedCommunity Community { get; private set; }
		/// <summary>
		/// Whether the picture is adult or safe. Similar to <see cref="Safety"/>.
		/// </summary>
		[JsonProperty("rating")]
		public string Rating { get; private set; }
		/// <summary>
		/// The tags associated with the post.
		/// </summary>
		[JsonProperty("tags")]
		public string Tags { get; private set; }
		/// <summary>
		/// Who owns this image.
		/// </summary>
		[JsonProperty("copyright")]
		public DeviantArtOEmbedCopyright Copyright { get; private set; }
		/// <summary>
		/// The image's width.
		/// </summary>
		[JsonProperty("width")]
		public string Width { get; private set; }
		/// <summary>
		/// The image's height.
		/// </summary>
		[JsonProperty("height")]
		public string Height { get; private set; }
		/// <summary>
		/// File type, with no period in front of it.
		/// </summary>
		[JsonProperty("imagetype")]
		public string Imagetype { get; private set; }
		/// <summary>
		/// The url to the thumbnail.
		/// </summary>
		[JsonProperty("thumbnail_url")]
		public Uri ThumbnailUrl { get; private set; }
		/// <summary>
		/// The thumbnail's width.
		/// </summary>
		[JsonProperty("thumbnail_width")]
		public int ThumbnailWidth { get; private set; }
		/// <summary>
		/// The thumbnail's height.
		/// </summary>
		[JsonProperty("thumbnail_height")]
		public int ThumbnailHeight { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(ImageUrl));
		}
	}
}