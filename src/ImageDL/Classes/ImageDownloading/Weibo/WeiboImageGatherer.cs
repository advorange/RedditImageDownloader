using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Weibo
{
	/// <summary>
	/// Gathers images from a specified Weibo link.
	/// </summary>
	public struct WeiboImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("weibo.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
		{
			return await WeiboPostDownloader.GetWeiboImagesAsync(client, url).CAF();
		}
	}
}