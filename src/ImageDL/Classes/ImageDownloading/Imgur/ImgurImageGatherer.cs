using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Enums;
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
				return new ImageResponse(FailureReason.Success, null, new Uri(u));
			}
			var images = await ImgurImageDownloader.GetImagesFromApi(client, u.Split('/').Last()).CAF();
			if (images.Any())
			{
				var tasks = images.Select(async x => await x.GetImagesAsync(client).CAF());
				var urls = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x.ImageUrls).ToArray();
				return new ImageResponse(FailureReason.Success, null, urls);
			}
			return new ImageResponse(FailureReason.NotFound, $"Unable to find any images for {url}.", url);
		}
	}
}