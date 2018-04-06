using System;
using System.Threading;
using System.Threading.Tasks;
using ImageDL.Classes.SettingParsing;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can download images.
	/// </summary>
	public interface IImageDownloader
	{
		/// <summary>
		/// Used to set arguments via command line.
		/// </summary>
		SettingParser SettingParser { get; }
		/// <summary>
		/// Indicates that all arguments have been set and that the user wants the downloader to start.
		/// </summary>
		bool CanStart { get; }
		/// <summary>
		/// The domain name the downloader works for.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		/// <param name="services">Holds the services. Should at least hold a downloader client.</param>
		/// <param name="token">Cancellation token for a semaphore slim that makes sure only one instance of downloading is happening.</param>
		/// <returns>An awaitable task which downloads images.</returns>
		Task StartAsync(IServiceProvider services, CancellationToken token = default);
	}
}