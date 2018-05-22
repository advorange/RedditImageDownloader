using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru.Yandere
{
	/// <summary>
	/// Gathers images from a specified Yandere link.
	/// </summary>
	public struct YandereImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("yande.re");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await YanderePostDownloader.GetYandereImagesAsync(client, url).CAF();
		}
	}
}