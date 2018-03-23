using ImageDL.Classes.ImageGatherers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can download images.
	/// </summary>
	public interface IImageDownloader : INotifyPropertyChanged
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
		int MaxImageSimilarity { get; set; }
		/// <summary>
		/// How many images to cache per thread. Lower = faster, but more CPU.
		/// </summary>
		int ImagesCachedPerThread { get; set; }
		/// <summary>
		/// The minimum score an image can have before it won't be downloaded. Not every site uses this.
		/// </summary>
		int MinScore { get; set; }
		/// <summary>
		/// Indicates whether or not to add already saved images to the cache before downloading images.
		/// </summary>
		bool CompareSavedImages { get; set; }
		/// <summary>
		/// Indicates whether or not to print extra information to the console. Such as variables being set.
		/// </summary>
		bool Verbose { get; set; }
		/// <summary>
		/// Indicates whether or not to create the directory if it does not exist.
		/// </summary>
		bool CreateDirectory { get; set; }
		/// <summary>
		/// Returns true if all arguments (aside from ones with default values) have been set at least once.
		/// </summary>
		bool AllArgumentsSet { get; }
		/// <summary>
		/// Returns true when images are being downloaded.
		/// </summary>
		bool BusyDownloading { get; }
		/// <summary>
		/// Returns true after all images have been downloaded.
		/// </summary>
		bool DownloadsFinished { get; }
		/// <summary>
		/// How to scrape specific websites.
		/// </summary>
		List<WebsiteScraper> Scrapers { get; }
		/// <summary>
		/// The oldest allowed posts.
		/// </summary>
		DateTime OldestAllowed { get; }
		/// <summary>
		/// The comparer to use for images.
		/// </summary>
		IImageComparer ImageComparer { get; set; }

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		/// <param name="token">Cancellation token for a semaphore slim that makes sure only one instance of downloading is happening.</param>
		/// <returns>An awaitable task which downloads images.</returns>
		Task StartAsync(CancellationToken token = default);
		/// <summary>
		/// Sets values with passed in values parsed from strings.
		/// </summary>
		/// <param name="args">The arguments to set.</param>
		void SetArguments(string[] args);
		/// <summary>
		/// Asks for unset arguments which are necessary for the downloader to start.
		/// </summary>
		void AskForArguments();
	}
}
