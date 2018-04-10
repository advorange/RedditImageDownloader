using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru
{
	/// <summary>
	/// Gathers images from a specified Gelbooru link.
	/// </summary>
	public sealed class GelbooruImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("gelbooru.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await GelbooruImageDownloader.GetGelbooruImagesAsync(client, url).CAF();
		}
	}
}