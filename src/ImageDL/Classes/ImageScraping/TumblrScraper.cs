using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageScraping
{
	/*
	/// <summary>
	/// Scrapes images from tumblr.com.
	/// </summary>
	public sealed class TumblrScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"^(?!.*(media\.tumblr)).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <inheritdoc />
		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("tumblr.com");
		}
		/// <inheritdoc />
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		/// <inheritdoc />
		public override Uri EditUri(Uri uri)
		{
			//If blog post will throw exception but gets caught when downloading
			return new Uri(RemoveQuery(uri).ToString().Replace("/post/", "/image/"));
		}
		/// <inheritdoc />
		protected override Task<ImagesResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants().Any(x => x.GetAttributeValue("id", null) == "safemode_actions_display"))
			{
				return Task.FromResult(new ImagesResult(uri, false, this, Enumerable.Empty<Uri>(), "this tumblr post is locked behind safemode"));
			}

			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return Task.FromResult(new ImagesResult(uri, false, this, Convert(images.Select(x => x.GetAttributeValue("content", null))), null));
		}
	}*/
}