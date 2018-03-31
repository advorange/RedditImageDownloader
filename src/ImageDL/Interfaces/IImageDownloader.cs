using ImageDL.Classes;
using ImageDL.Classes.ImageComparing;
using ImageDL.Classes.SettingParsing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can download images.
	/// </summary>
	public interface IImageDownloader
	{
		/// <summary>
		/// The directory to save images to.
		/// </summary>
		string Directory { get; set; }
		/// <summary>
		/// The amount of posts to look through.
		/// </summary>
		int AmountToDownload { get; set; }
		/// <summary>
		/// The minimum width an image can have before it won't be downloaded.
		/// </summary>
		int MinWidth { get; set; }
		/// <summary>
		/// The minimum height an image can have before it won't be downloaded.
		/// </summary>
		int MinHeight { get; set; }
		/// <summary>
		/// The maximum age an image can have before it won't be downloaded.
		/// </summary>
		int MaxDaysOld { get; set; }
		/// <summary>
		/// The maximum allowed image similarity before an image is considered a duplicate.
		/// </summary>
		Percentage MaxImageSimilarity { get; set; }
		/// <summary>
		/// How many images to cache per thread. Lower = faster, but more CPU.
		/// </summary>
		int ImagesCachedPerThread { get; set; }
		/// <summary>
		/// The minimum score an image can have before it won't be downloaded. Not every site uses this.
		/// </summary>
		int MinScore { get; set; }
		/// <summary>
		/// The minimum aspect ratio an image can have.
		/// </summary>
		AspectRatio MinAspectRatio { get; set; }
		/// <summary>
		/// The maximum aspect ratio an image can have.
		/// </summary>
		AspectRatio MaxAspectRatio { get; set; }
		/// <summary>
		/// Indicates whether or not to add already saved images to the cache before downloading images.
		/// </summary>
		bool CompareSavedImages { get; set; }
		/// <summary>
		/// Indicates whether or not to create the directory if it does not exist.
		/// </summary>
		bool CreateDirectory { get; set; }
		/// <summary>
		/// Indicates the user wants the downloader to start.
		/// </summary>
		bool Start { get; set; }
		/// <summary>
		/// The comparer to use for images.
		/// </summary>
		ImageComparer ImageComparer { get; set; }
		/// <summary>
		/// Used to set arguments via command line.
		/// </summary>
		SettingParser SettingParser { get; set; }
		/// <summary>
		/// The oldest allowed posts.
		/// </summary>
		DateTime OldestAllowed { get; }
		/// <summary>
		/// Indicates that all arguments have been set and that the user wants the downloader to start.
		/// </summary>
		bool CanStart { get; }

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		/// <param name="token">Cancellation token for a semaphore slim that makes sure only one instance of downloading is happening.</param>
		/// <returns>An awaitable task which downloads images.</returns>
		Task StartAsync(CancellationToken token = default);
	}
}
