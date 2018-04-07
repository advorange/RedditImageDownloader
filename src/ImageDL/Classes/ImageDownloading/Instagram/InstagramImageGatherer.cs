using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Enums;
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
			var search = "/p/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await InstagramImageDownloader.GetInstagramPostAsync(client, id).CAF();
				if (post != null)
				{
					return await post.GetImagesAsync(client).CAF();
				}
				return new ImageResponse(FailureReason.NotFound, $"Unable to find any images for {url}.", url);
			}
			return new ImageResponse(FailureReason.Misc, null, url);
		}
	}
}