using AdvorangesUtils;
using ImageDL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Gathers images from a specified Instagram link.
	/// </summary>
	public sealed class InstagramImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("instagram.com");
		}
		/// <inheritdoc />
		public async Task<ImagesResult> GetImagesAsync(ImageDownloaderClient client, Uri uri)
		{
			var u = client.RemoveQuery(uri).ToString();
			var search = "/p/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await InstagramImageDownloader.GetInstagramPostAsync(client, id).CAF();
				return post == null
					? ImagesResult.FromNotFound(uri)
					: ImagesResult.FromGatherer(uri, new[] { new Uri(post.ContentUrl) });
			}
			return ImagesResult.FromMisc(uri);
		}
	}
}
