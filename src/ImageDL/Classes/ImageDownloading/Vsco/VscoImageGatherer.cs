using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Enums;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Gathers images from a specified Vsco link.
	/// </summary>
	public sealed class VscoImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("vsco.co");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			var search = "/media/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await VscoImageDownloader.GetVscoPostAsync(client, id).CAF();
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