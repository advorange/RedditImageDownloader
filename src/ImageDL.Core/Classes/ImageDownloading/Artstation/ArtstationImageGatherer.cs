using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Artstation
{
	/// <summary>
	/// Gathers images from the specified Artstation link.
	/// </summary>
	public struct ArtstationImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("artstation.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await ArtstationPostDownloader.GetArtstationImagesAsync(client, url).CAF();
	}
}