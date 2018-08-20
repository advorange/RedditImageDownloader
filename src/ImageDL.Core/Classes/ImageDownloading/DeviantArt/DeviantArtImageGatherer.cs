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
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await DeviantArtPostDownloader.GetDeviantArtImagesAsync(client, url).CAF();
		}
	}
}