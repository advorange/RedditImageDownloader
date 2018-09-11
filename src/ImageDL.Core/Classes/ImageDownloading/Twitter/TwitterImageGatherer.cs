using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Twitter
{
	/// <summary>
	/// Gathers images from a specified Twitter link.
	/// </summary>
	public struct TwitterImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("twitter.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await TwitterPostDownloader.GetTwitterImagesAsync(client, url).CAF();
	}
}