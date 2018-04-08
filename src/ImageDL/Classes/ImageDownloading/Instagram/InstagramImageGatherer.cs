using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Instagram.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Instagram
{
	/// <summary>
	/// Gathers images from a specified Instagram link.
	/// </summary>
	public sealed class InstagramImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("instagram.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/p/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				if (await InstagramImageDownloader.GetInstagramPostAsync(client, id).CAF() is InstagramMediaNode post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}