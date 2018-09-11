using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Pinterest
{
	/// <summary>
	/// Gathers images from a specified Pinterest link.
	/// </summary>
	public struct PinterestImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("pinterest.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await PinterestPostDownloader.GetPinterestImagesAsync(client, url).CAF();
	}
}