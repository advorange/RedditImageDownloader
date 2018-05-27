using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Diyidan
{
	/// <summary>
	/// Gathers images from a specified Diyidan link.
	/// </summary>
	public struct DiyidanImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("diyidan.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await DiyidanPostDownloader.GetDiyidanImagesAsync(client, url).CAF();
		}
	}
}