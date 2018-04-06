using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using ImageDL.Classes.ImageComparing;
using Media = System.Windows.Media;

namespace ImageDL.Windows
{
	/// <summary>
	/// Holds details about images which have been downloaded, while using a Windows specific implementation of <see cref="GenerateThumbnailHash(Stream, int)"/>.
	/// </summary>
	public sealed class WindowsImageComparer : ImageComparer
	{
		private static readonly Media.PixelFormat PIXEL_FORMAT = Media.PixelFormats.Bgra32;

		/// <inheritdoc />
		protected override IEnumerable<bool> GenerateThumbnailHash(Stream s, int thumbnailSize)
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
			fcbm.DestinationFormat = PIXEL_FORMAT;
			fcbm.EndInit();
			fcbm.Freeze();

			//Copy the image's data to a new array
			//Mostly gotten from here https://social.msdn.microsoft.com/Forums/vstudio/en-US/82a5731e-e201-4aaf-8d4b-062b138338fe/getting-pixel-information-from-a-bitmapimage?forum=wpf
			var pixelSize = PIXEL_FORMAT.BitsPerPixel / 8;
			var stride = fcbm.PixelWidth * pixelSize;
			var bytes = new byte[fcbm.PixelHeight * stride];
			fcbm.CopyPixels(bytes, stride, 0);

			var brightnesses = new List<float>();
			for (var y = 0; y < fcbm.PixelHeight; ++y)
			{
				for (var x = 0; x < fcbm.PixelWidth; ++x)
				{
					var index = y * stride + x * pixelSize;
					var r = bytes[index];
					var g = bytes[index + 1];
					var b = bytes[index + 2];
					var a = bytes[index + 3];
					//Magic numbers for caclulating brightness, see: https://stackoverflow.com/a/596243
					brightnesses.Add((0.299f * r + 0.587f * g + 0.114f * b) * (a / 255f));
				}
			}
			var avgBrightness = brightnesses.Average();
			return brightnesses.Select(x => x > avgBrightness);
		}
	}
}