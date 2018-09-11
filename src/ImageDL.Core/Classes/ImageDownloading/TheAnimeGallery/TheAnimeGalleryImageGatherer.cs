using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.TheAnimeGallery
{
	/// <summary>
	/// Gathers images from a specified TheAnimeGallery link.
	/// </summary>
	public struct TheAnimeGalleryImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("theanimegallery.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await TheAnimeGalleryPostDownloader.GetTAGImagesAsync(client, url).CAF();
	}
}