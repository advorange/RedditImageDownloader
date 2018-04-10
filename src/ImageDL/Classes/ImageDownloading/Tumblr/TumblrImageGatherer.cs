using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Tumblr
{
	/// <summary>
	/// Gathers images from a specified Tumblr link.
	/// </summary>
	public sealed class TumblrImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("tumblr.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await TumblrImageDownloader.GetTumblrImagesAsync(client, url).CAF();
		}
	}
}