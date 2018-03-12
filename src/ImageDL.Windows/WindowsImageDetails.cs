using ImageDL.Classes.ImageComparers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ImageDL.Windows
{
	/// <summary>
	/// Holds details about an image which has been downloaded.
	/// </summary>
	public sealed class WindowsImageDetails : ImageDetails
	{
		//For caclulating brightness, source: https://stackoverflow.com/a/596243
		private const float RED_WEIGHTING = 0.299f;
		private const float GREEN_WEIGHTING = 0.587f;
		private const float BLUE_WEIGHTING = 0.114f;

		protected override ImmutableArray<bool> GenerateThumbnailHash(Stream s, int thumbnailSize)
		{
			using (var bm = GenerateThumbnail(s, thumbnailSize))
			{
				//Source: https://stackoverflow.com/a/19586876
				//Lock the image once (GetPixel locks and unlocks a lot of times, it's quicker to do it this way)
				var size = new Rectangle(Point.Empty, new Size(bm.Width, bm.Height));
				var data = bm.LockBits(size, ImageLockMode.ReadOnly, bm.PixelFormat);

				var pixelSize = Image.GetPixelFormatSize(data.PixelFormat) / 8;
				if (pixelSize != 4)
				{
					throw new NotSupportedException("invalid image format: must be argb32bpp or able to be converted to that");
				}
				var padding = data.Stride - (data.Width * pixelSize);

				//Copy the image's data to a new array
				var bytes = new byte[data.Height * data.Stride];
				Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

				var brightnesses = new List<float>();
				var totalBrightness = 0f;
				var index = 0;
				for (var y = 0; y < Height; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						var a = bytes[index + 3];
						var r = bytes[index + 2];
						var g = bytes[index + 1];
						var b = bytes[index];
						var brightness = (RED_WEIGHTING * r + GREEN_WEIGHTING * g + BLUE_WEIGHTING * b) * (a / 255f);
						brightnesses.Add(brightness);
						totalBrightness += brightness;
						index += pixelSize;
					}
					index += padding;
				}
				var avgBrightness = totalBrightness / brightnesses.Count;
				return brightnesses.Select(x => x > avgBrightness).ToImmutableArray();
			}
		}

		private Bitmap GenerateThumbnail(Stream s, int thumbnailSize)
		{
			s.Seek(0, SeekOrigin.Begin);

			//Create an image with a small size
			var bmi = new BitmapImage
			{
				CreateOptions = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile,
			};
			bmi.BeginInit();
			bmi.StreamSource = s;
			bmi.DecodePixelWidth = thumbnailSize;
			bmi.DecodePixelHeight = thumbnailSize;
			bmi.EndInit();
			bmi.Freeze();

			//Convert the image format to argb32bpp so the pixel size will always be 4
			var fcbm = new FormatConvertedBitmap();
			fcbm.BeginInit();
			fcbm.Source = bmi;
			fcbm.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
			fcbm.EndInit();
			fcbm.Freeze();

			using (var ms = new MemoryStream())
			{
				//Convert the small argb32bpp image to a memory stream
				//So that ms can then be used in a bitmap to get all its pixels easily
				var enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(fcbm));
				enc.Save(ms);

				return new Bitmap(ms);
			}
		}
	}
}
