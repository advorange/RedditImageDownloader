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
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("pinterest.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await PinterestImageDownloader.GetPinterestImagesAsync(client, url).CAF();
		}
	}
}