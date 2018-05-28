using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImageDL.Attributes;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Weibo
{
	/// <summary>
	/// Downloads images from Weibo.
	/// </summary>
	[DownloaderName("Weibo")]
	public sealed class WeiboPostDownloader : PostDownloader
	{
		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetWeiboImagesAsync(IDownloaderClient client, Uri url)
		{
			throw new NotImplementedException();
		}
	}
}