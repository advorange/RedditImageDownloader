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
		public async Task<GatheredImagesResponse> GetImagesAsync(ImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString().Replace("_d", "");
			var code = u.Split('/').Last();
			//Galleries may be albums, and we can tell if their code is 5 letters long
			if ((u.CaseInsContains("/a/") || u.CaseInsContains("/gallery/")))
			{
				if (code.Length == 5)
				{
					var images = await ImgurImageDownloader.GetImagesAsync(client, code).CAF();
					return images.Any()
						? GatheredImagesResponse.FromGatherer(url, images.SelectMany(x => x.ContentUrls).ToArray())
						: GatheredImagesResponse.FromNotFound(url);
				}
				else if (code.Length == 7)
				{
					return GatheredImagesResponse.FromImage(new Uri($"https://i.imgur.com/{code}.png"));
				}
			}
			return GatheredImagesResponse.FromUnknown(url);
		}
	}
}