using System;
using System.IO;

namespace ImageDL.Classes.ImageDownloading.Tumblr
{
	/// <summary>
	/// Gathers images from a specified Tumblr link.
	/// </summary>
	public sealed class TumblrImageGatherer
	{
		/// <summary>
		/// Gets the link to the full size image.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static Uri GetFullSizeImage(Uri url)
		{
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