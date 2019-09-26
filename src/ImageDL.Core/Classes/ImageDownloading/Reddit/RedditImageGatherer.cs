using System;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Reddit
{
	/// <summary>
	/// Gathers images from a specified Reddit link.
	/// </summary>
	public struct RedditImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> RedditPostDownloader.GetRedditImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("reddit.com");
	}
}