using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageDL.Classes.ImageComparing;

namespace ImageDL
{
	/// <summary>
	/// Holds details about images which have been downloaded, while using a Windows specific implementation of <see cref="HashImageStream(Stream, int)"/>.
	/// </summary>
	/// <remarks>
	/// This is roughly 4x slower than <see cref="Classes.ImageComparing.Implementations.ImageSharpImageComparer"/> but uses 10x less memory.
	/// </remarks>
	public sealed class NetFrameworkImageComparer : ImageComparer
	{
		/// <summary>
		/// Creates an instance of <see cref="NetFrameworkImageComparer"/>
		/// </summary>
		/// <param name="databasePath"></param>
		public NetFrameworkImageComparer(string databasePath) : base(databasePath) { }

		/// <inheritdoc />
		protected override string HashImageStream(Stream s, int size)
		{
			s.Seek(0, SeekOrigin.Begin);

			//Create an image with a small size
			var bmi = new BitmapImage
			{
				CreateOptions = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile,
			};
			bmi.BeginInit();
			bmi.StreamSource = s;
			bmi.DecodePixelWidth = size;
			bmi.DecodePixelHeight = size;
			bmi.EndInit();
			bmi.Freeze();

			//Convert the image format to argb32bpp so the pixel size will always be 4
			var fcbm = new FormatConvertedBitmap();
			fcbm.BeginInit();
			fcbm.Source = bmi;
			fcbm.DestinationFormat = PixelFormats.Bgra32;
			fcbm.EndInit();
			fcbm.Freeze();

			//Copy the image's data to a new array
			//Mostly gotten from here https://social.msdn.microsoft.com/Forums/vstudio/en-US/82a5731e-e201-4aaf-8d4b-062b138338fe/getting-pixel-information-from-a-bitmapimage?forum=wpf
			var pixelSize = PixelFormats.Bgra32.BitsPerPixel / 8;
			var stride = fcbm.PixelWidth * pixelSize;
			var bytes = new byte[fcbm.PixelHeight * stride];
			fcbm.CopyPixels(bytes, stride, 0);

			var brightnesses = new float[fcbm.PixelHeight * fcbm.PixelWidth];
			var currentPixel = 0;
			switch (pixelSize)
			{
				case 4: //RGBA
					for (int y = 0; y < fcbm.PixelHeight; ++y)
					{
						for (int x = 0; x < fcbm.PixelWidth; ++x)
						{
							var index = y * stride + x * pixelSize;
							var r = bytes[index];
							var g = bytes[index + 1];
							var b = bytes[index + 2];
							var a = bytes[index + 3];
							brightnesses[currentPixel] = CalculateBrightness(a, r, g, b);
							++currentPixel; //Could do currentPixel++ above, but that's less clear
						}
					}
					break;

				case 3: //RGB
					for (int y = 0; y < fcbm.PixelHeight; ++y)
					{
						for (int x = 0; x < fcbm.PixelWidth; ++x)
						{
							var index = y * stride + x * pixelSize;
							var r = bytes[index];
							var g = bytes[index + 1];
							var b = bytes[index + 2];
							brightnesses[currentPixel] = CalculateBrightness(255, r, g, b);
							++currentPixel;
						}
					}
					break;

				default:
					throw new NotSupportedException("The default implementation of this method only supports 32 and 24 bit pixels.");
			}
			return BrightnessesToString(brightnesses);
		}
	}
}