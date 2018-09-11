using System;
using System.Collections.Generic;
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
		public string Hash { get; internal set; }
		/// <summary>
		/// The name of the file. This does not involve the directory.
		/// </summary>
		[BsonField("FileName")]
		public string FileName { get; internal set; }
		/// <inheritdoc />
		[BsonField("Width")]
		public int Width { get; internal set; }
		/// <inheritdoc />
		[BsonField("Height")]
		public int Height { get; internal set; }
		/// <summary>
		/// The hash of the image's thumbnail in boolean form (1 means light, 0 means dark).
		/// </summary>
		[BsonField("HashedThumbnail")]
		public string HashedThumbnail { get; internal set; }

		/// <summary>
		/// Creates an instance of <see cref="ImageDetails"/>.
		/// </summary>
		internal ImageDetails() { }
		/// <summary>
		/// Creates an instance of <see cref="ImageDetails"/>.
		/// </summary>
		/// <param name="md5">The hash of the file's contents.</param>
		/// <param name="fileName">The file's name.</param>
		/// <param name="width">The width of the image.</param>
		/// <param name="height">The height of the image.</param>
		/// <param name="hashedThumbnail">The hash of the image's pixels.</param>
		internal ImageDetails(string md5, string fileName, int width, int height, string hashedThumbnail)
		{
			Hash = md5;
			FileName = fileName;
			Width = width;
			Height = height;
			HashedThumbnail = hashedThumbnail;
		}

		/// <summary>
		/// Returns true if the percentage of elements which match are greater than or equal to <paramref name="matchPercentage"/>.
		/// </summary>
		/// <param name="other">The details to compare to.</param>
		/// <param name="matchPercentage">The valid percentage for matching.</param>
		/// <returns>Returns a boolean indicating whether or not the hashes match.</returns>
		/// <exception cref="InvalidOperationException">Occurs when the thumbnail sizes do not match.</exception>
		public bool Equals(ImageDetails other, Percentage matchPercentage)
		{
			if (HashedThumbnail.Length != other.HashedThumbnail.Length)
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
			for (int i = 0; i < HashedThumbnail.Length; ++i)
			{
				if (HashedThumbnail[i] == other.HashedThumbnail[i])
				{
					++matchCount;
				}
			}
			return (matchCount / (float)HashedThumbnail.Length) >= matchPercentage.Value;
		}
		/// <summary>
		/// Returns the file name.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => FileName;
	}
}