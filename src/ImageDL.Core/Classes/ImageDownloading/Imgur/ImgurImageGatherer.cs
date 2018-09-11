using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Gathers images from a specified Imgur link.
	/// </summary>
	public struct ImgurImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("imgur.com");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await ImgurPostDownloader.GetImgurImagesAsync(client, url).CAF();
	}
}