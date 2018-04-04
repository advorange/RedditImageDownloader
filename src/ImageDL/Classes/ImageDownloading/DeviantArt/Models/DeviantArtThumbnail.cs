namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models
{
	/// <summary>
	/// Thumbnails for an image.
	/// </summary>
	public struct DeviantArtThumbnail
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

		internal DeviantArtThumbnail(string source, int width, int height, bool isTransparent)
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
