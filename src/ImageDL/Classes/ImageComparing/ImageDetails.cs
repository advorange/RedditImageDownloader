using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace ImageDL.Classes.ImageComparing
{
	/// <summary>
	/// Holds information about an image.
	/// </summary>
	public sealed class ImageDetails
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

		internal ImageDetails(Uri uri, FileInfo file, int width, int height, IEnumerable<bool> hashedThumbnail)
		{
			Uri = uri;
			File = file;
			Width = width;
			Height = height;
			HashedThumbnail = hashedThumbnail.ToImmutableArray();
			ThumbnailSize = (int)Math.Ceiling(Math.Sqrt(HashedThumbnail.Length));
		}

		/// <summary>
		/// Returns true if the percentage of elements which match are greater than or equal to <paramref name="matchPercentage"/>.
		/// </summary>
		/// <param name="other">The details to compare to.</param>
		/// <param name="matchPercentage">The valid percentage for matching.</param>
		/// <returns>Returns a boolean indicating whether or not the hashes match.</returns>
		/// <exception cref="InvalidOperationException">Occurs when the thumbnail sizes do not match.</exception>
		/// <exception cref="ArgumentException">Occurs when either image details are not initialized correctly.</exception>
		public bool Equals(ImageDetails other, Percentage matchPercentage)
		{
			if (ThumbnailSize != other.ThumbnailSize)
			{
				throw new InvalidOperationException("The thumbnails must be the same size when checking equality.");
			}

			//If the aspect ratio is too different then don't bother checking the hash
			var margin = 1 - matchPercentage.Value;
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
			return (matchCount / (float)(ThumbnailSize * ThumbnailSize)) >= matchPercentage.Value;
		}
		/// <summary>
		/// Attempts to delete either this or the other image details. Whichever is lower res will be deleted and then returned.
		/// </summary>
		/// <param name="other">The other image details to compare and delete.</param>
		/// <returns></returns>
		public ImageDetails DetermineWhichToDelete(ImageDetails other)
		{
			//Delete/remove whatever is the smaller image
			var firstPix = Width * Height;
			var secondPix = other.Width * other.Height;
			//If each image has the same pixel count then go by file creation time, if not then just go by pixel count
			return (firstPix == secondPix ? File?.CreationTimeUtc < other.File?.CreationTimeUtc : firstPix < secondPix) ? this : other;
		}
	}
}