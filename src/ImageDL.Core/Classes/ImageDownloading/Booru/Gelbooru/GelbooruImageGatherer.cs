using System;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru.Gelbooru
{
	/// <summary>
	/// Gathers images from a specified Gelbooru link.
	/// </summary>
	public struct GelbooruImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> GelbooruPostDownloader.GetGelbooruImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("gelbooru.com");
	}
}