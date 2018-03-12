using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class InstagramScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"(\/p\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("instagram.com");
		}
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		public override Uri EditUri(Uri uri)
		{
			return RemoveQuery(uri);
		}
		protected override Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
		{
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return Task.FromResult(new ScrapeResult(images.Select(x => x.GetAttributeValue("content", null)), null));
		}
	}
}
