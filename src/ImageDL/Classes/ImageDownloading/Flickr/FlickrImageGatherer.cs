using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Flickr
{
	/// <summary>
	/// Gathers images from a specified Flickr link.
	/// </summary>
	public struct FlickrImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("flickr.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await FlickrPostDownloader.GetFlickrImagesAsync(client, url).CAF();
		}
	}
}