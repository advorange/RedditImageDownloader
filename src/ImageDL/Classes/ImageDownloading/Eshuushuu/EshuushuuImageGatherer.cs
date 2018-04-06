using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Gathers images from a specified Eshuushuu link.
	/// </summary>
	public sealed class EshuushuuImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("e-shuushuu.net");
		}
		/// <inheritdoc />
		public async Task<GatheredImagesResponse> GetImagesAsync(ImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			var search = "/image/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await EshuushuuImageDownloader.GetEshuushuuPostAsync(client, id).CAF();
				return post == null
					? GatheredImagesResponse.FromNotFound(url)
					: GatheredImagesResponse.FromGatherer(url, post.ContentUrls.ToArray());
			}
			return GatheredImagesResponse.FromUnknown(url);
		}
	}
}