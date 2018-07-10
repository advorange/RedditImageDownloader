using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;

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
			byte[] bytes;
			using (var img = Image.Load<Rgba32>(s))
			{
				img.Mutate(x =>
				{
					x.Resize(ThumbnailSize, ThumbnailSize);
				});
				bytes = img.SavePixelData();
			}

			return HashBytes(bytes, 4, ThumbnailSize, ThumbnailSize);
		}
	}
}