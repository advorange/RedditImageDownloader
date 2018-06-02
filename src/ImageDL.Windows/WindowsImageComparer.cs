using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageDL.Classes.ImageComparing;

namespace ImageDL.Windows
{
	/// <summary>
	/// Holds details about images which have been downloaded, while using a Windows specific implementation of <see cref="HashImageStream(Stream, int)"/>.
	/// </summary>
	public sealed class WindowsImageComparer : ImageComparer
	{
		/// <summary>
		/// Creates an instance of <see cref="WindowsImageComparer"/> with the database path as a file in the current directory.
		/// </summary>
		public WindowsImageComparer() : this(Path.Combine(Directory.GetCurrentDirectory(), DATABASE_NAME)) { }
		/// <summary>
		/// Creates an instance of <see cref="WindowsImageComparer"/>
		/// </summary>
		/// <param name="databasePath"></param>
		public WindowsImageComparer(string databasePath) : base(databasePath) { }

		/// <inheritdoc />
		protected override string HashImageStream(Stream s)
		{
			s.Seek(0, SeekOrigin.Begin);

			//Create an image with a small size
			var bmi = new BitmapImage
			{
				CreateOptions = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile,
			};
			bmi.BeginInit();
			bmi.StreamSource = s;
			bmi.DecodePixelWidth = ThumbnailSize;
			bmi.DecodePixelHeight = ThumbnailSize;
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

			return HashBytes(bytes, pixelSize, ThumbnailSize, ThumbnailSize);
		}
	}
}