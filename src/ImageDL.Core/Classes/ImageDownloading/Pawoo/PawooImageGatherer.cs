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
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await PawooPostDownloader.GetPawooImagesAsync(client, url).CAF();
		}
	}
}