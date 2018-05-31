using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageDL.Classes.ImageComparing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

namespace ImageDL.Test
{
	/// <summary>
	/// Holds details about images which have been downloaded, while using ImageSharp for <see cref="GetThumbnailBytes(Stream, int)"/>.
	/// </summary>
	/// <remarks>This uses a high amount of RAM, but is quicker than the Windows implementation.</remarks>
	public sealed class ImageSharpImageComparer : ImageComparer
	{
		/// <summary>
		/// Creates an instance of <see cref="ImageSharpImageComparer"/> with the database path as a file in the current directory.
		/// </summary>
		public ImageSharpImageComparer() : this(Path.Combine(Directory.GetCurrentDirectory(), DATABASE_NAME)) { }
		/// <summary>
		/// Creates an instance of <see cref="ImageSharpImageComparer"/>
		/// </summary>
		/// <param name="databasePath"></param>
		public ImageSharpImageComparer(string databasePath) : base(databasePath) { }

		/// <inheritdoc />
		protected override string GetThumbnailBytes(Stream s, int thumbnailSize)
		{
			s.Seek(0, SeekOrigin.Begin);
			
			//Not sure if there is a better way to do this or not
			//The speed is good, but the memory usage is really high
			byte[] bytes;
			using (var img = Image.Load<Rgba32>(s))
			{
				img.Mutate(x =>
				{
					x.Resize(thumbnailSize, thumbnailSize);
				});
				bytes = img.SavePixelData();
			}

			return HashThumbnailBytes(bytes, 4, thumbnailSize, thumbnailSize);
		}
	}
}