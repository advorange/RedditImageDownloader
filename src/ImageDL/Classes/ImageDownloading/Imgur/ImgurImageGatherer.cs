using AdvorangesUtils;
using ImageDL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
	/// <summary>
	/// Gathers images from a specified Imgur link.
	/// </summary>
	public sealed class ImgurImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("imgur");
		}
		/// <inheritdoc />
		public async Task<ImagesResult> GetImagesAsync(ImageDownloaderClient client, Uri uri)
		{
			var u = client.RemoveQuery(uri).ToString().Replace("_d", "");
			var code = u.Split('/').Last();
			//Galleries may be albums, and we can tell if their code is 5 letters long
			if ((u.CaseInsContains("/a/") || u.CaseInsContains("/gallery/")) && code.Length == 5)
			{
				var images = await ImgurImageDownloader.GetImagesAsync(client, code).CAF();
				return images.Any()
					? ImagesResult.FromGatherer(uri, images.Select(x => new Uri(x.ContentUrl)))
					: ImagesResult.FromNotFound(uri);
			}
			else
			{
				return ImagesResult.FromMisc(uri);
			}
		}
	}
}
