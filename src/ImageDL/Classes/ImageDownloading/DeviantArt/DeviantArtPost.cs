using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Model for similarities between scraped posts and posts gotten through the API.
	/// </summary>
	public sealed class DeviantArtPost : Post
	{
		/// <summary>
		/// Whether or not the post is NSFW.
		/// </summary>
		public readonly bool IsMature;
		/// <summary>
		/// The width of the image.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		public readonly int Height;
		/// <summary>
		/// The direct image link.
		/// </summary>
		public readonly string Source;
		/// <summary>
		/// The author's username.
		/// </summary>
		public readonly string AuthorUsername;
		/// <summary>
		/// The author's user icon url.
		/// </summary>
		public readonly string AuthorUserIcon;
		/// <summary>
		/// The author's UUID.
		/// </summary>
		public readonly string AuthorUUID;
		/// <summary>
		/// Various sizes of the image.
		/// </summary>
		public readonly List<Thumbnail> Thumbnails;

		private readonly string _PostId;
		private readonly int _Favorites;

		/// <inheritdoc />
		public override string Link => $"https://www.fav.me/{Id}";
		/// <inheritdoc />
		public override string Id => _PostId;
		/// <inheritdoc />
		public override int Score => _Favorites;

		internal DeviantArtPost(DeviantArtApiPost api)
		{
			IsMature = api.IsMature;
			_PostId = api.Url.Split('-').Last();
			Width = api.Content.Width;
			Height = api.Content.Height;
			_Favorites = api.Stats.Favorites;
			Source = api.Content.Source;
			AuthorUsername = api.Author.Username;
			AuthorUserIcon = api.Author.UserIcon;
			AuthorUUID = api.Author.UUID;
			Thumbnails = api.Thumbnails?.Select(x => new Thumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<Thumbnail>();
		}
		internal DeviantArtPost(DeviantArtScrappedPost scrape)
		{
			IsMature = scrape.IsMature;
			_PostId = scrape.Id.ToString();
			Width = scrape.Width;
			Height = scrape.Height;
			_Favorites = -1;
			Source = scrape.Source;
			AuthorUsername = scrape.Author.Username;
			AuthorUserIcon = scrape.Author.UserIcon;
			AuthorUUID = scrape.Author.UUID;
			Thumbnails = scrape.Sizes?.Select(x => new Thumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<Thumbnail>();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}

	/// <summary>
	/// Thumbnails for an image.
	/// </summary>
	public struct Thumbnail
	{
		/// <summary>
		/// Is the thumbnail see through?
		/// </summary>
		public readonly bool IsTransparent;
		/// <summary>
		/// The width of the thumbnail.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The height of the thumbnail.
		/// </summary>
		public readonly int Height;
		/// <summary>
		/// The link to the thumbnail.
		/// </summary>
		public readonly string Source;

		internal Thumbnail(string source, int width, int height, bool isTransparent)
		{
			IsTransparent = isTransparent;
			Width = width;
			Height = height;
			Source = source;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}