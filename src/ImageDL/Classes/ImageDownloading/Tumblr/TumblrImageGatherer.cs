using System;
using System.IO;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Tumblr.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Tumblr
{
	/// <summary>
	/// Gathers images from a specified Tumblr link.
	/// </summary>
	public sealed class TumblrImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("tumblr.com");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = GetFullSizeImage(ImageDownloaderClient.RemoveQuery(url)).ToString().Replace("/post/", "/image/");
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/image/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var username = url.Host.Split('.')[0];
				var id = u.Substring(index + search.Length).Split('/')[0];
				if (await TumblrImageDownloader.GetTumblrPostAsync(client, username, id).CAF() is TumblrPost post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
		/// <summary>
		/// Gets the link to the full size image.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static Uri GetFullSizeImage(Uri url)
		{
			//TODO: move the static methods from image gatherers and downloaders to utils classes
			//Can't get the raw for inline, and static.tumblr is already full size because they're used for themes.
			if (url.AbsolutePath.Contains("inline") || url.Host.Contains("static.tumblr"))
			{
				return url;
			}
			if (url.Host.Contains("media.tumblr"))
			{
				//Example:
				//https://78.media.tumblr.com/475ede973aab130576a77789c82925b9/tumblr_p5xxjlVAK91td53jko1_1280.jpg
				//https://a.tumblr.com/475ede973aab130576a77789c82925b9/tumblr_p5xxjlVAK91td53jko1_raw.jpg
				var parts = url.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				var file = parts[1];
				var raw = $"{file.Substring(0, file.LastIndexOf('_'))}_raw{Path.GetExtension(file)}";
				return new Uri($"https://a.tumblr.com/{parts[0]}/{raw}");
			}
			return url; //Didn't fit into any of the above, guess just return it?
		}
	}
}