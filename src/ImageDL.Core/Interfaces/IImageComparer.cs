using ImageDL.Classes.ImageComparing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can compare images.
	/// </summary>
	public interface IImageComparer : INotifyPropertyChanged
	{
		/// <summary>
		/// The amount of images the comparer currently has stored.
		/// </summary>
		int StoredImages { get; }
		/// <summary>
		/// The amount of images which have already been checked for duplicates.
		/// </summary>
		int CurrentImagesSearched { get; }
		/// <summary>
		/// The size of the thumbnail. Bigger = more accurate, but slowness grows at n^2.
		/// </summary>
		int ThumbnailSize { get; set; }

		/// <summary>
		/// Attempts to cache the image.
		/// </summary>
		/// <param name="uri">The location of the image.</param>
		/// <param name="file">The file the image is saved to or will be saved to.</param>
		/// <param name="stream">The image's data.</param>
		/// <param name="width">The width of the original image.</param>
		/// <param name="height">The height of the original image.</param>
		/// <param name="error">If there are any problems with trying to cache the file.</param>
		/// <returns></returns>
		bool TryStore(Uri uri, FileInfo file, Stream stream, int width, int height, out string error);
		/// <summary>
		/// Attempts to create image details from a file.
		/// </summary>
		/// <param name="file">The file to cache.</param>
		/// <param name="thumbnailSize">The size to create the thumbnail.</param>
		/// <param name="md5Hash">The hash of the image's stream.</param>
		/// <param name="details">The details of the image.</param>
		/// <returns></returns>
		bool TryCreateImageDetailsFromFile(FileInfo file, int thumbnailSize, out string md5Hash, out ImageDetails details);
		/// <summary>
		/// Attempts to cache files which have already been saved.
		/// </summary>
		/// <param name="directory">The directory to cache files from.</param>
		/// <param name="imagesPerThread">How many images to cache per thread. Lower = faster, but more CPU/Disk usage</param>
		/// <param name="token">The token used to cancel caching files.</param>
		/// <returns></returns>
		Task CacheSavedFilesAsync(DirectoryInfo directory, int imagesPerThread, CancellationToken token = default);
		/// <summary>
		/// Checks each image against every other image in order to detect duplicates.
		/// </summary>
		/// <param name="matchPercentage">How close an image can be percentage wise before being considered a duplicate.</param>
		void DeleteDuplicates(float matchPercentage);
		/// <summary>
		/// Generates a hash where true = light, false = dark. Used in comparing images for mostly similar instead of exactly similar.
		/// </summary>
		/// <param name="s">The image's data.</param>
		/// <param name="thumbnailSize">The size to make the image.</param>
		/// <returns>The image's hash.</returns>
		IEnumerable<bool> GenerateThumbnailHash(Stream s, int thumbnailSize);
	}
}
