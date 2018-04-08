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
			var u = ImageDownloaderClient.RemoveQuery(url).ToString().Replace("_d", "");
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var id = u.Split('/').Last();
			var images = await ImgurImageDownloader.GetImagesFromApi(client, id).CAF();
			if (images.Any())
			{
				var tasks = images.Select(async x => await x.GetImagesAsync(client).CAF());
				var urls = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x.ImageUrls).ToArray();
				return ImageResponse.FromImages(urls);
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}