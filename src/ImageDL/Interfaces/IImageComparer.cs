using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageDL.Classes;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can compare images.
	/// </summary>
	public interface IImageComparer
	{
		/// <summary>
		/// Attempts to cache the image.
		/// </summary>
		/// <param name="file">The file the image is saved to or will be saved to.</param>
		/// <param name="stream">The image's data.</param>
		/// <param name="width">The width of the original image.</param>
		/// <param name="height">The height of the original image.</param>
		/// <param name="error">If there are any problems with trying to cache the file.</param>
		/// <returns></returns>
		bool TryStore(FileInfo file, Stream stream, int width, int height, out string error);
		/// <summary>
		/// Attempts to cache files which have already been saved.
		/// </summary>
		/// <param name="directory">The directory to cache files from.</param>
		/// <param name="imagesPerThread">How many images to cache per thread. Lower = faster, but more CPU/Disk usage</param>
		/// <param name="token">The token used to cancel caching files.</param>
		/// <returns>The amount of images successfully cached.</returns>
		Task<int> CacheSavedFilesAsync(DirectoryInfo directory, int imagesPerThread, CancellationToken token = default);
		/// <summary>
		/// Checks each image against every other image in order to detect duplicates.
		/// After all duplicates are found they are removed from the passed in directory.
		/// </summary>
		/// <param name="directory">The directory to check for duplicates.</param>
		/// <param name="matchPercentage">How close an image can be percentage wise before being considered a duplicate.</param>
		/// <returns>The amount of duplicates deleted.</returns>
		int HandleDuplicates(DirectoryInfo directory, Percentage matchPercentage);
	}
}