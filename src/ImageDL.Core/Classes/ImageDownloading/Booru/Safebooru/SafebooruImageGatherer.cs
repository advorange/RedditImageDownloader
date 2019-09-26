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
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> SafebooruPostDownloader.GetSafebooruImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("safebooru.org");
	}
}