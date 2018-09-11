using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Gathers images from a specified Instagram link.
	/// </summary>
	public struct InstagramImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("instagram.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await InstagramPostDownloader.GetInstagramImagesAsync(client, url).CAF();
	}
}