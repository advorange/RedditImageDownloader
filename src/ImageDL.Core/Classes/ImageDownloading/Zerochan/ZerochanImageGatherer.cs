using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Zerochan
{
	/// <summary>
	/// Gathers images from a specified Zerochan link.
	/// </summary>
	public struct ZerochanImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("zerochan.net");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await ZerochanPostDownloader.GetZerochanImagesAsync(client, url).CAF();
		}
	}
}