using System;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Gathers images from a specified Konachan link.
	/// </summary>
	public struct KonachanImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> KonachanPostDownloader.GetKonachanImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("konachan.com");
	}
}