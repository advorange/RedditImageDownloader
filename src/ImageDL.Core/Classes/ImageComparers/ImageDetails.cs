using ImageDL.Utilities;
using System;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;

namespace ImageDL.Classes.ImageComparers
{
	public abstract class ImageDetails
	{
		/// <summary>
		/// The location of the source of the image.
		/// </summary>
		public Uri Uri { get; private set; }
		/// <summary>
		/// The location the image was saved to.
		/// </summary>
		public FileInfo File { get; private set; }
		/// <summary>
		/// The image's width;
		/// </summary>
		public int Width { get; private set; }
		/// <summary>
		/// The image's height;
		/// </summary>
		public int Height { get; private set; }
		/// <summary>
		/// The hash of the image's thumbnail in boolean form.
		/// </summary>
		public ImmutableArray<bool> HashedThumbnail { get; private set; }
		/// <summary>
		/// The size of the image's thumbnail (the thumbnail is a square).
		/// </summary>
		public int ThumbnailSize { get; private set; }

		/// <summary>
		/// Creates the boolean hash for <see cref="HashedThumbnail"/> and sets all the variables.
		/// </summary>
		/// <param name="uri">The source of the image.</param>
		/// <param name="file">The location the image was saved to.</param>
		/// <param name="s">The image stream.</param>
		/// <param name="thumbnailSize">The size to make the thumbnail.</param>
		/// <exception cref="NotSupportedException">Occurs when the image format is not argb32bpp or something which can be converted to that.</exception>
		/// <exception cref="ArgumentException">When the stream length is less than 1.</exception>
		public static T Create<T>(Uri uri, FileInfo file, Stream s, Image image, int thumbnailSize) where T : ImageDetails, new()
		{
			if (image == null)
			{
				throw new ArgumentException("Image cannot be null.", nameof(image));
			}

			var details = new T
			{
				Uri = uri,
				File = file,
				ThumbnailSize = thumbnailSize,
				Width = image.Width,
				Height = image.Height,
			};
			details.HashedThumbnail = details.GenerateThumbnailHash(s, thumbnailSize);
			return details;
		}
		/// <summary>
		/// Attempts to create <see cref="ImageDetails"/> from a file.
		/// </summary>
		/// <param name="file">The file to read.</param>
		/// <param name="thumbnailSize">The size to make the thumbnail.</param>
		/// <param name="md5Hash">The image's data hash.</param>
		/// <param name="details">The image's pixel hash.</param>
		/// <returns>A boolean indicating whether or not <paramref name="details"/> were created successfully.</returns>
		public static bool TryCreateFromFile<T>(FileInfo file, int thumbnailSize, out string md5Hash, out T details) where T : ImageDetails, new()
		{
			md5Hash = null;
			details = default;

			//The second check is because for some reason file.Exists will be true when the file does NOT exist
			if (!file.FullName.IsImagePath() || !System.IO.File.Exists(file.FullName))
			{
				return false;
			}

			try
			{
				using (var s = file.OpenRead())
				using (var img = Image.FromStream(s, false, false))
				{
					md5Hash = s.MD5Hash();
					details = Create<T>(new Uri(file.FullName), file, s, img, thumbnailSize);
					return true;
				}
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// Returns true if the percentage of elements which match are greater than or equal to <paramref name="percentageForSimilarity"/>.
		/// </summary>
		/// <param name="other">The details to compare to.</param>
		/// <param name="percentageForSimilarity">The valid percentage for matching.</param>
		/// <returns>Returns a boolean indicating whether or not the hashes match.</returns>
		/// <exception cref="InvalidOperationException">Occurs when the thumbnail sizes do not match.</exception>
		/// <exception cref="ArgumentException">Occurs when either image details are not initialized correctly.</exception>
		public bool Equals(ImageDetails other, float percentageForSimilarity)
		{
			if (ThumbnailSize != other.ThumbnailSize)
			{
				throw new InvalidOperationException("The thumbnails must be the same size when checking equality.");
			}

			//If the aspect ratio is too different then don't bother checking the hash
			var margin = 1 - percentageForSimilarity;
			var otherAR = other.Width / (float)other.Height;
			var thisAR = Width / (float)Height;
			if (otherAR > thisAR * (1 + margin) || otherAR < thisAR * (1 - margin))
			{
				return false;
			}

			var matchCount = 0;
			for (int i = 0; i < (ThumbnailSize * ThumbnailSize); ++i)
			{
				if (HashedThumbnail[i] == other.HashedThumbnail[i])
				{
					++matchCount;
				}
			}
			return (matchCount / (float)(ThumbnailSize * ThumbnailSize)) >= percentageForSimilarity;
		}

		/// <summary>
		/// Get the thumbnail hash so these can be more accurately compared.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="thumbnailSize"></param>
		/// <returns></returns>
		protected abstract ImmutableArray<bool> GenerateThumbnailHash(Stream s, int thumbnailSize);
	}
}