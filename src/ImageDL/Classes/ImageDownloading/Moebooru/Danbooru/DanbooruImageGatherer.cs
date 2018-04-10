using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Danbooru
{
	/// <summary>
	/// Gathers images from a specified Danbooru link.
	/// </summary>
	public sealed class DanbooruImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("donmai.us");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await DanbooruImageDownloader.GetDanbooruImagesAsync(client, url).CAF();
		}
	}
}