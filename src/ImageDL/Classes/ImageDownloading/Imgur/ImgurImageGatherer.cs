using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Gathers images from a specified Imgur link.
	/// </summary>
	public sealed class ImgurImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("imgur.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await ImgurImageDownloader.GetImgurImagesAsync(client, url).CAF();
		}
	}
}