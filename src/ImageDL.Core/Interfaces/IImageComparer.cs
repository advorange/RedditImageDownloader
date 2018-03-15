using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	/// <summary>
	/// Interface for something that can compare images.
	/// </summary>
	public interface IImageComparer : INotifyPropertyChanged
	{
		/// <summary>
		/// The amount of images the comparer currently has stroed.
		/// </summary>
		int StoredImages { get; }
		/// <summary>
		/// The amount of images which have already been checked for duplicates.
		/// </summary>
		int CurrentImagesSearched { get; }
		/// <summary>
		/// The size of the thumbnail. Bigger = more accurate, but slowness grows at n^2.
		/// </summary>
		int ThumbnailSize { get; }

		/// <summary>
		/// Attempts to cache the image.
		/// </summary>
		/// <param name="uri">The location of the image.</param>
		/// <param name="file">The file the image is saved to or will be saved to.</param>
		/// <param name="stream">The image's data.</param>
		/// <param name="minWidth">The minimum acceptable width for the image.</param>
		/// <param name="minHeight">The minimum acceptable height for the image.</param>
		/// <param name="error">If there are any problems with trying to cache the file.</param>
		/// <returns></returns>
		bool TryStore(Uri uri, FileInfo file, Stream stream, int minWidth, int minHeight, out string error);
		/// <summary>
		/// Attempts to cache files which have already been saved.
		/// </summary>
		/// <param name="directory">The directory to cache files from.</param>
		/// <param name="taskGroupLength">How many images to cache per thread. Lower = faster, but more CPU/Disk usage</param>
		/// <returns></returns>
		Task CacheSavedFilesAsync(DirectoryInfo directory, int taskGroupLength);
		/// <summary>
		/// Checks each image against every other image in order to detect duplicates.
		/// </summary>
		/// <param name="matchPercentage">How close an image can be percentage wise before being considered a duplicate.</param>
		void DeleteDuplicates(float matchPercentage);
	}
}
