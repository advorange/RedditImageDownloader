using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Pixiv
{
	/// <summary>
	/// Gathers images from a specified Pixiv link.
	/// </summary>
	public struct PixivImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("pixiv.net");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await PixivPostDownloader.GetPixivImagesAsync(client, url).CAF();
	}
}