using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru.Danbooru
{
	/// <summary>
	/// Gathers images from a specified Danbooru link.
	/// </summary>
	public struct DanbooruImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("donmai.us");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await DanbooruPostDownloader.GetDanbooruImagesAsync(client, url).CAF();
	}
}