using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Safebooru
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
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await SafebooruImageDownloader.GetSafebooruImagesAsync(client, url);
		}
	}
}