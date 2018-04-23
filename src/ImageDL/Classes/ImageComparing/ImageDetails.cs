using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageDL.Interfaces;
using LiteDB;

namespace ImageDL.Classes.ImageComparing
{
	/// <summary>
	/// Holds information about an image.
	/// </summary>
	public sealed class ImageDetails : ISize
	{
		/// <summary>
		/// The Md5 hash of the image. This is used as a key in the database.
		/// </summary>
		[BsonId, BsonField("Hash")]
		public string Hash { get; set; }
		/// <summary>
		/// The name of the file. This does not involve the directory.
		/// </summary>
		[BsonField("FileName")]
		public string FileName { get; set; }
		/// <inheritdoc />
		[BsonField("Width")]
		public int Width { get; set; }
		/// <inheritdoc />
		[BsonField("Height")]
		public int Height { get; set; }
		/// <summary>
		/// The hash of the image's thumbnail in boolean form.
		/// </summary>
		[BsonField("HashedThumbnail")]
		public IList<bool> HashedThumbnail { get; set; }
		/// <summary>
		/// The size of the image's thumbnail (the thumbnail is a square).
		/// </summary>
		[BsonIgnore]
		public int ThumbnailSize => (int)Math.Ceiling(Math.Sqrt(HashedThumbnail.Count));

		internal ImageDetails() { }
		internal ImageDetails(string md5, string fileName, int width, int height, IEnumerable<bool> hashedThumbnail)
		{
			Hash = md5;
			FileName = fileName;
			Width = width;
			Height = height;
			HashedThumbnail = hashedThumbnail.ToList();
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
		/// Returns the file name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return FileName;
		}
	}
}