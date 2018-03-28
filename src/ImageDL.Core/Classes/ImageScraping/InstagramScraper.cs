using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageScraping
{
	/// <summary>
	/// Scrapes images from instagram.com.
	/// </summary>
	public sealed class InstagramScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"(\/p\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <inheritdoc />
		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("instagram.com");
		}
		/// <inheritdoc />
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		/// <inheritdoc />
		public override Uri EditUri(Uri uri)
		{
			return RemoveQuery(uri);
		}
		/// <inheritdoc />
		protected override Task<ScrapeResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			//TODO: change cause I think this doesn't work anymore?
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return Task.FromResult(new ScrapeResult(uri, false, this, Convert(images.Select(x => x.GetAttributeValue("content", null))), null));
		}
	}
}