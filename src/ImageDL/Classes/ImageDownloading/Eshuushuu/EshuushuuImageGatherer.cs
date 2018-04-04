using AdvorangesUtils;
using ImageDL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Gathers images from a specified Eshuushuu link.
	/// </summary>
	public sealed class EshuushuuImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("e-shuushuu.net");
		}
		/// <inheritdoc />
		public async Task<ImagesResult> GetImagesAsync(ImageDownloaderClient client, Uri uri)
		{
			var u = client.RemoveQuery(uri).ToString();
			var search = "/image/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await EshuushuuImageDownloader.GetEshuushuuPostAsync(client, id).CAF();
				return post == null
					? ImagesResult.FromNotFound(uri)
					: ImagesResult.FromGatherer(uri, new[] { new Uri(post.ContentUrl) });
			}
			return ImagesResult.FromMisc(uri);
		}
	}
}
