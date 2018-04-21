using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Yandere
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
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await YandereImageDownloader.GetYandereImagesAsync(client, url).CAF();
		}
	}
}