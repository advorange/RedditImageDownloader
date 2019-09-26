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
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> WeiboPostDownloader.GetWeiboImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("weibo.com");
	}
}