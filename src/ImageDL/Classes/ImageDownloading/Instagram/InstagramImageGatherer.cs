using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Gathers images from a specified Instagram link.
	/// </summary>
	public sealed class InstagramImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("instagram.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await InstagramImageDownloader.GetInstagramImagesAsync(client, url).CAF();
		}
	}
}