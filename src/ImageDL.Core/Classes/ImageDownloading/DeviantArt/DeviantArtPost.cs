using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Model for similarities between scraped posts and posts gotten through the API.
	/// </summary>
	public class DeviantArtPost
	{
		/// <summary>
		/// Whether or not the post is NSFW.
		/// </summary>
		public readonly bool IsMature;
		/// <summary>
		/// The xth posted Deviation. This is not the UUID.
		/// </summary>
		public readonly int PostId;
		/// <summary>
		/// The width of the image.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The height of the image.
		/// </summary>
		public readonly int Height;
		/// <summary>
		/// How many people have favorited it.
		/// </summary>
		public readonly int Favorites;
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

		internal DeviantArtPost(ApiDeviantArtResults.ApiDeviantArtPost api)
		{
			IsMature = api.IsMature;
			PostId = Convert.ToInt32(api.Url.Split('-').Last());
			Width = api.Content.Width;
			Height = api.Content.Height;
			Favorites = api.Stats.Favorites;
			Source = api.Content.Source;
			AuthorUsername = api.Author.Username;
			AuthorUserIcon = api.Author.UserIcon;
			AuthorUUID = api.Author.UUID;
			Thumbnails = api.Thumbnails?.Select(x => new Thumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<Thumbnail>();
		}
		internal DeviantArtPost(ScrapedDeviantArtPost scrape)
		{
			IsMature = scrape.IsMature;
			PostId = scrape.Id;
			Width = scrape.Width;
			Height = scrape.Height;
			Favorites = -1;
			Source = scrape.Source;
			AuthorUsername = scrape.Author?.Username;
			AuthorUserIcon = scrape.Author?.UserIcon;
			AuthorUUID = scrape.Author?.UUID;
			Thumbnails = scrape.Sizes?.Select(x => new Thumbnail(x.Source, x.Width, x.Height, x.IsTransparent))?.ToList() ?? new List<Thumbnail>();
		}

		/// <summary>
		/// Thumbnails for an image.
		/// </summary>
		public class Thumbnail
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
		}
	}
}
