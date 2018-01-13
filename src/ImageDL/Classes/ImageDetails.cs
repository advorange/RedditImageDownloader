using ImageDL.Utilities;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;

namespace ImageDL.Classes
{
	/// <summary>
	/// Holds details about an image which has been downloaded.
	/// </summary>
	public struct ImageDetails
	{
		/// <summary>
		/// The location of the source of the image.
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
		public readonly ImmutableArray<bool> HashedThumbnail;
		/// <summary>
		/// The size of the image's thumbnail (the thumbnail is a square).
		/// </summary>
		public readonly int ThumbnailSize;
		/// <summary>
		/// Indicates whether or not the struct was created correctly.
		/// </summary>
		public readonly bool InitializedCorrectly;

		/// <summary>
		/// Creates the boolean hash for <see cref="HashedThumbnail"/> and sets all the variables.
		/// </summary>
		/// <param name="uri">The source of the image.</param>
		/// <param name="file">The location the image was saved to.</param>
		/// <param name="s">The image stream.</param>
		/// <param name="thumbnailSize">The size to make the thumbnail.</param>
		/// <exception cref="NotSupportedException">Occurs when the image format is not argb32bpp or something which can be converted to that.</exception>
		public ImageDetails(Uri uri, FileInfo file, Stream s, int thumbnailSize)
		{
			Uri = uri;
			File = file;

			//Make sure that the stream can be read fully
			s.Seek(0, SeekOrigin.Begin);

			var decoder = BitmapDecoder.Create(s, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
			Width = decoder.Frames[0].PixelWidth;
			Height = decoder.Frames[0].PixelHeight;
			//Reset the stream after the decoder has read it
			s.Seek(0, SeekOrigin.Begin);

			//Create an image with a small size
			var bmi = new BitmapImage
			{
				CreateOptions = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile,
			};
			try
			{
				bmi.BeginInit();
				bmi.StreamSource = s;
				bmi.DecodePixelWidth = thumbnailSize;
				bmi.DecodePixelHeight = thumbnailSize;
				bmi.EndInit();
				bmi.Freeze();
			}
			//Comes from EndInit()
			//Not sure why but it would always happen on a file called Sergeant_Stubby 2.jpg
			catch (FileFormatException ffe)
			{
				ffe.Write();
				HashedThumbnail = new ImmutableArray<bool>();
				ThumbnailSize = 0;
				InitializedCorrectly = false;
				return;
			}

			//Convert the image format to argb32bpp so the pixel size will always be 4
			var fcbm = new FormatConvertedBitmap();
			fcbm.BeginInit();
			fcbm.Source = bmi;
			fcbm.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
			fcbm.EndInit();
			fcbm.Freeze();

			var brightnesses = new List<float>();
			var totalBrightness = 0f;
			using (var ms = new MemoryStream())
			{
				//Convert the small argb32bpp image to a memory stream
				//So that ms can then be used in a bitmap to get all its pixels easily
				var enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(fcbm));
				enc.Save(ms);

				//Create a square thumbnail
				using (var thumb = new Bitmap(ms))
				{
					//Source: https://stackoverflow.com/a/19586876
					//Lock the image once (GetPixel locks and unlocks a lot of times, it's quicker to do it this way)
					var data = thumb.LockBits(new Rectangle(Point.Empty, new Size(thumb.Width, thumb.Height)), ImageLockMode.ReadOnly, thumb.PixelFormat);
					var pixelSize = Image.GetPixelFormatSize(data.PixelFormat) / 8;
					var padding = data.Stride - (data.Width * pixelSize);

					//Copy the image's data to a new array
					var bytes = new byte[data.Height * data.Stride];
					Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

					if (pixelSize != 4)
					{
						throw new NotSupportedException("invalid image format: must be argb32bpp or able to be converted to that");
					}

					var index = 0;
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
			}
			var avgBrightness = totalBrightness / brightnesses.Count;

			HashedThumbnail = brightnesses.Select(x => x > avgBrightness).ToImmutableArray();
			ThumbnailSize = thumbnailSize;
			InitializedCorrectly = true;
		}

		/// <summary>
		/// Attempts to create <see cref="ImageDetails"/> from a file.
		/// </summary>
		/// <param name="file">The file to read.</param>
		/// <param name="thumbnailSize">The size to make the thumbnail.</param>
		/// <param name="md5Hash">The image's data hash.</param>
		/// <param name="details">The image's pixel hash.</param>
		/// <returns>A boolean indicating whether or not <paramref name="details"/> were created successfully.</returns>
		public static bool TryCreateFromFile(FileInfo file, int thumbnailSize, out string md5Hash, out ImageDetails details)
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
				{
					md5Hash = s.Hash<MD5>();
					details = new ImageDetails(new Uri(file.FullName), file, s, thumbnailSize);
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
			else if (!InitializedCorrectly)
			{
				throw new ArgumentException("not initialized correctly", "this");
			}
			else if (!other.InitializedCorrectly)
			{
				throw new ArgumentException("not initialized correctly", nameof(other));
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
	}
}
