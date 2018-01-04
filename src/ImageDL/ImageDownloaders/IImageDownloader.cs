using ImageDL.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Abstraction of <see cref="ImageDownloader{TPost}"/>.
	/// </summary>
	public interface IImageDownloader
	{
		/// <summary>
		/// Fires when all arguments have been set.
		/// </summary>
		event Func<Task> AllArgumentsSet;
		/// <summary>
		/// Fires after all images have been downloaded.
		/// </summary>
		event Func<Task> DownloadsFinished;
		/// <summary>
		/// Returns true between when all arguments have been set and images start downloading.
		/// </summary>
		bool IsReady { get; }
		/// <summary>
		/// Returns true after all images have been downloaded.
		/// </summary>
		bool IsDone { get; }

		string Directory { get; set; }
		int AmountToDownload { get; set; }
		int MinWidth { get; set; }
		int MinHeight { get; set; }
		int MaxDaysOld { get; set; }
		int MaxImageSimilarity { get; set; }
		bool CompareSavedImages { get; set; }

		/// <summary>
		/// Start downloading images.
		/// </summary>
		/// <returns>An asynchronous task which downloads images.</returns>
		Task StartAsync();
		/// <summary>
		/// Gives help to users.
		/// </summary>
		/// <param name="argNames">The names of arguments users need help with.</param>
		void GiveHelp(params string[] argNames);
		/// <summary>
		/// Sets arguments with only text/user input.
		/// </summary>
		/// <param name="args">The text to set.</param>
		void SetArguments(params string[] args);
		/// <summary>
		/// Prints to the console what arguments are still needed.
		/// </summary>
		void AskForArguments();
		/// <summary>
		/// Saves links to a file.
		/// </summary>
		/// <param name="contentLinks">The links to save.</param>
		/// <param name="file">The file to save to.</param>
		void SaveContentLinks(IEnumerable<ContentLink> contentLinks, FileInfo file);
	}
}
