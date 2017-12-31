using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web;

namespace ImageDL.Classes
{
	/// <summary>
	/// Holds details about an image which has been downloaded.
	/// </summary>
	public struct ImageDetails
	{
		/// <summary>
		/// The location the image was downloaded from.
		/// </summary>
		public readonly Uri Uri;
		/// <summary>
		/// The location the image was saved to.
		/// </summary>
		public readonly FileInfo File;
		/// <summary>
		/// The source image's width.
		/// </summary>
		public readonly int Width;
		/// <summary>
		/// The source image's height.
		/// </summary>
		public readonly int Height;
		/// <summary>
		/// The hash of the image's thumbnail in boolean form.
		/// </summary>
		public readonly IEnumerable<bool> HashedThumbnail;
		/// <summary>
		/// The size of the image's thumbnail (the thumbnail is a square).
		/// </summary>
		public readonly int ThumbnailSize;

		public ImageDetails(Uri uri, FileInfo file, Bitmap bm, int thumbnailSize)
		{
			Uri = uri;
			File = file;
			Width = bm.Width;
			Height = bm.Height;

			//Source: https://stackoverflow.com/a/19586876
			var brightnesses = new List<float>();
			var totalBrightness = 0f;
			//Create a square thumbnail
			using (var thumb = new Bitmap(bm, thumbnailSize, thumbnailSize))
			{
				//Lock the image once (GetPixel locks and unlocks a lot of times, it's quicker to do it this way)
				var data = thumb.LockBits(new Rectangle(Point.Empty, new Size(thumb.Width, thumb.Height)), ImageLockMode.ReadOnly, thumb.PixelFormat);
				var pixelSize = data.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
				var padding = data.Stride - (data.Width * pixelSize);

				//Copy the image's data to a new array
				var bytes = new byte[data.Height * data.Stride];
				Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

				var index = 0;
				if (pixelSize == 4)
				{
					for (var y = 0; y < data.Height; y++)
					{
						for (var x = 0; x < data.Width; x++)
						{
							var a = bytes[index + 3];
							var r = bytes[index + 2];
							var g = bytes[index + 1];
							var b = bytes[index];
							var brightness = (0.299f * r + 0.587f * g + 0.114f * b) * (a / 255f);
							brightnesses.Add(brightness);
							totalBrightness += brightness;
							index += pixelSize;
						}

						index += padding;
					}
				}
				else
				{
					for (var y = 0; y < data.Height; y++)
					{
						for (var x = 0; x < data.Width; x++)
						{
							var r = bytes[index + 2];
							var g = bytes[index + 1];
							var b = bytes[index];
							var brightness = (0.299f * r + 0.587f * g + 0.114f * b);
							brightnesses.Add(brightness);
							totalBrightness += brightness;
							index += pixelSize;
						}

						index += padding;
					}
				}
			}
			var avgBrightness = totalBrightness / brightnesses.Count;

			HashedThumbnail = brightnesses.Select(x => x > avgBrightness).ToImmutableList();
			ThumbnailSize = thumbnailSize;
		}

		/// <summary>
		/// Attempts to create <see cref="ImageDetails"/> from a file.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="thumbnailSize"></param>
		/// <param name="md5Hash"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public static bool TryCreateFromFile(FileInfo file, int thumbnailSize, out string md5Hash, out ImageDetails details)
		{
			if (!MimeMapping.GetMimeMapping(file.FullName).StartsWith("image/"))
			{
				md5Hash = null;
				details = default;
				return false;
			}

			try
			{
				using (var s = file.OpenRead())
				using (var bm = new Bitmap(s))
				{
					md5Hash = s.Hash<MD5>();
					details = new ImageDetails(new Uri(file.FullName), file, bm, thumbnailSize);
					return true;
				}
			}
			catch
			{
				md5Hash = null;
				details = default;
				return false;
			}
		}

		/// <summary>
		/// Returns true if the percentage of elements which match are greater than or equal to <paramref name="percentageForSimilarity"/>.
		/// </summary>
		/// <param name="other">The details to compare to.</param>
		/// <param name="percentageForSimilarity">The valid percentage for matching.</param>
		/// <returns>Returns a boolean indicating whether or not the hashes match.</returns>
		public bool Equals(ImageDetails other, float percentageForSimilarity)
		{
			//If the aspect ratio is too different then don't bother checking the hash
			var margin = 1 - percentageForSimilarity;
			var otherAR = other.Width / (float)other.Height;
			var thisAR = Width / (float)Height;
			if (otherAR > thisAR * (1 + margin) || otherAR < thisAR * (1 - margin))
			{
				return false;
			}

			var matchCount = 0;
			for (int i = 0; i < HashedThumbnail.Count(); ++i)
			{
				if (HashedThumbnail.ElementAt(i) == other.HashedThumbnail.ElementAt(i))
				{
					++matchCount;
				}
			}
			return (matchCount / (float)(ThumbnailSize * ThumbnailSize)) >= percentageForSimilarity;
		}
	}
}
