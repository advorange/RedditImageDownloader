using System;
using System.IO;
using System.Linq;

using AdvorangesUtils;

using MetadataExtractor;

namespace ImageDL.Core.Utilities
{
	/// <summary>
	/// What to do when duplicate sizes are found in image metadata.
	/// </summary>
	public enum DuplicateSizeMethod
	{
		/// <summary>
		/// Return the smallest values of each.
		/// </summary>
		Minimum,

		/// <summary>
		/// Return the largest values of each.
		/// </summary>
		Maximum,

		/// <summary>
		/// Return the first values of each.
		/// </summary>
		First,

		/// <summary>
		/// Return the last values of each.
		/// </summary>
		Last,

		/// <summary>
		/// Throw an exception.
		/// </summary>
		Throw,
	}

	/// <summary>
	/// Utilities for images.
	/// </summary>
	public static class ImageUtils
	{
		/// <summary>
		/// Gets the width and height of an image through metadata.
		/// If multiple widths and heights are gotten via metadata, will return the smallest ones.
		/// Will not seek on the stream, so make sure it's at the beginning yourself.
		/// </summary>
		/// <param name="s">The image's data.</param>
		/// <param name="method">What to do if more than one width or height is found.</param>
		/// <returns></returns>
		public static (int Width, int Height) GetImageSize(this Stream s, DuplicateSizeMethod method = DuplicateSizeMethod.Minimum)
		{
			try
			{
				var tags = ImageMetadataReader.ReadMetadata(s).SelectMany(x => x.Tags);
				var width = tags.SelectWhere(x => x.Name == "Image Width", x => Convert.ToInt32(x.Description.Split(' ')[0]));
				var height = tags.SelectWhere(x => x.Name == "Image Height", x => Convert.ToInt32(x.Description.Split(' ')[0]));
				if (width.Count() > 1 || height.Count() > 1)
				{
					switch (method)
					{
						case DuplicateSizeMethod.Minimum:
							return (width.Min(), height.Min());

						case DuplicateSizeMethod.Maximum:
							return (width.Max(), height.Max());

						case DuplicateSizeMethod.First:
							return (width.First(), height.First());

						case DuplicateSizeMethod.Last:
							return (width.Last(), height.Last());

						case DuplicateSizeMethod.Throw:
							throw new InvalidOperationException("More than one width or height was gotten for the image.");
					}
				}
				return (width.Single(), height.Single());
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Unable to parse the image's width and height from the stream's metadata.", e);
			}
		}
	}
}