using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan
{
	/// <summary>
	/// Gathers images from a specified Konachan link.
	/// </summary>
	public struct KonachanImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("konachan.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await KonachanImageDownloader.GetKonachanImagesAsync(client, url).CAF();
		}
	}
}