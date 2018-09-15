using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageDL.Classes.ImageComparing.Implementations
{
	/// <summary>
	/// Holds details about images which have been downloaded, while using ImageSharp for <see cref="HashImageStream(Stream)"/>.
	/// </summary>
	/// <remarks>This uses a high amount of RAM, but is quicker than the Windows implementation.</remarks>
	public sealed class ImageSharpImageComparer : ImageComparer
	{
		/// <summary>
		/// Creates an instance of <see cref="ImageSharpImageComparer"/>
		/// </summary>
		/// <param name="databasePath"></param>
		public ImageSharpImageComparer(string databasePath) : base(databasePath) { }

		/// <inheritdoc />
		protected override string HashImageStream(Stream s)
		{
			s.Seek(0, SeekOrigin.Begin);

			//Not sure if there is a better way to do this or not
			//The speed is good, but the memory usage is really high
			Span<Rgba32> pixels;
			using (var img = Image.Load<Rgba32>(s))
			{
				img.Mutate(x => x.Resize(ThumbnailSize, ThumbnailSize));
				pixels = img.GetPixelSpan();
			}
			//GC.Collect helps lower the memory usage from around 2GB tops to 400MB tops, but I'm not sure if I know more than regular GC
#if false
			GC.Collect();
			GC.WaitForPendingFinalizers();
#endif

			var brightnesses = new float[pixels.Length];
			for (int i = 0; i < pixels.Length; ++i)
			{
				var pixel = pixels[i];
				brightnesses[i] = CalculateBrightness(pixel.A, pixel.R, pixel.G, pixel.B);
			}
			return BrightnessesToString(brightnesses);
		}
	}
}