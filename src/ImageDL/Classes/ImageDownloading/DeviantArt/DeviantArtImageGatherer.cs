using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Gathers images from a specified DeviantArt link.
	/// </summary>
	public struct DeviantArtImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("deviantart.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await DeviantArtImageDownloader.GetDeviantArtImagesAsync(client, url).CAF();
		}
	}
}