using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru.Safebooru
{
	/// <summary>
	/// Gathers images from a specified Safebooru link.
	/// </summary>
	public struct SafebooruImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("safebooru.org");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await SafebooruPostDownloader.GetSafebooruImagesAsync(client, url);
		}
	}
}