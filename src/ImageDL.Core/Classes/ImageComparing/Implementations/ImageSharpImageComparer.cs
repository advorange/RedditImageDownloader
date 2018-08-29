using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			var brightnesses = new List<float>();
			foreach (var pixel in pixels)
			{
				brightnesses.Add(CalculateBrightness(pixel.A, pixel.R, pixel.G, pixel.B));
			}
			var avgBrightness = brightnesses.Average();
			return new string(brightnesses.Select(x => x > avgBrightness ? '1' : '0').ToArray());
		}
	}
}