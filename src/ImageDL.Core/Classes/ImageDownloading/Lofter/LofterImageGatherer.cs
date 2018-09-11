using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Lofter
{
	/// <summary>
	/// Gathers images from a specified Lofter link.
	/// </summary>
	public struct LofterImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("lofter.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await LofterPostDownloader.GetLofterImagesAsync(client, url).CAF();
	}
}