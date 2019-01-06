using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImageDL.Classes;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can download images from a post.
	/// </summary>
	public interface IPostDownloader
	{
		/// <summary>
		/// Downloads all the images from the supplied posts.
		/// </summary>
		/// <param name="posts">The posts to download images from.</param>
		/// <param name="services">Holds the services. Must at least hold a <see cref="IDownloaderClient"/>.</param>
		/// <param name="token">Cancels downloading.</param>
		/// <returns>An awaitable task which downloads images.</returns>
		Task<DownloaderResponse> DownloadAsync(IEnumerable<IPost> posts, IServiceProvider services, CancellationToken token = default);
	}
}