using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api;
using ImageDL.Classes.ImageDownloading.DeviantArt.Models.Scraped;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models
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
		public readonly List<DeviantArtThumbnail> Thumbnails;

		private readonly string _PostId;
		private readonly string _Source;
		private readonly int _Favorites;

		/// <inheritdoc />
		public override string PostUrl => $"https://www.fav.me/{Id}";
		/// <inheritdoc />
		public override string ContentUrl => _Source;
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
			_Source = api.Content.Source;
			AuthorUsername = api.Author.Username;
			AuthorUserIcon = api.Author.UserIcon;
			AuthorUUID = api.Author.UUID;
			Thumbnails = api.Thumbnails?.Select(x => new DeviantArtThumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<DeviantArtThumbnail>();
		}
		internal DeviantArtPost(DeviantArtScrapedPost scrape)
		{
			IsMature = scrape.IsMature;
			_PostId = scrape.Id.ToString();
			Width = scrape.Width;
			Height = scrape.Height;
			_Favorites = -1;
			_Source = scrape.Source;
			AuthorUsername = scrape.Author.Username;
			AuthorUserIcon = scrape.Author.UserIcon;
			AuthorUUID = scrape.Author.UUID;
			Thumbnails = scrape.Sizes?.Select(x => new DeviantArtThumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<DeviantArtThumbnail>();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}