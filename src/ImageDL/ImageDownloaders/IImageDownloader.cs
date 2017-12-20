using System;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Abstraction of <see cref="ImageDownloader{TPost}"/>.
	/// </summary>
	public interface IImageDownloader
	{
		bool IsReady { get; }
		string Directory { get; set; }
		int AmountToDownload { get; set; }
		int MinWidth { get; set; }
		int MinHeight { get; set; }

		event EventHandler AllArgumentsSet;
		event EventHandler DownloadsFinished;

		/// <summary>
		/// Start downloading images.
		/// </summary>
		/// <returns>An asynchronous task which downloads images.</returns>
		Task StartAsync();
		/// <summary>
		/// Sets arguments with only text/user input.
		/// </summary>
		/// <param name="args">The text to set.</param>
		void AddArguments(string[] args);
		/// <summary>
		/// Prints to the console what arguments are still needed.
		/// </summary>
		void AskForArguments();
	}
}
