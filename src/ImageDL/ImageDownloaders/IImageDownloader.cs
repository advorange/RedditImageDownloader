using System;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Abstraction of <see cref="ImageDownloader{TPost}"/>.
	/// </summary>
	public interface IImageDownloader
	{
		string Directory { get; set; }
		int AmountToDownload { get; set; }
		int MinWidth { get; set; }
		int MinHeight { get; set; }
		int MaxDaysOld { get; set; }
		float MaxAcceptableImageSimilarity { get; set; }
		bool CompareWithAlreadySavedImages { get; set; }

		event Func<Task> AllArgumentsSet;
		event Func<Task> DownloadsFinished;

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
		void AddArguments(params string[] args);
		/// <summary>
		/// Prints to the console what arguments are still needed.
		/// </summary>
		void AskForArguments();
	}
}
