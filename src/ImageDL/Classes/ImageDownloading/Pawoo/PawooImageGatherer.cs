using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Pawoo
{
	/// <summary>
	/// Gathers images from a specified Pawoo link.
	/// </summary>
	public struct PawooImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("pawoo.net");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await PawooImageDownloader.GetPawooImagesAsync(client, url).CAF();
		}
	}
}