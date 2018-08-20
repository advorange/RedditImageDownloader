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
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("reddit.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await RedditPostDownloader.GetRedditImagesAsync(client, url).CAF();
		}
	}
}