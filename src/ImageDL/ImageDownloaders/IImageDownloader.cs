using System;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Abstraction of <see cref="ImageDownloader{TPost}"/>.
	/// </summary>
	public interface IImageDownloader
	{
		event Func<Task> AllArgumentsSet;
		event Func<Task> DownloadsFinished;
		bool IsReady { get; }

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
	}
}
