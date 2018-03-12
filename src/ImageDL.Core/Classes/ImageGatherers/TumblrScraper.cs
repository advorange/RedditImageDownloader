using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class TumblrScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"^(?!.*(media\.tumblr)).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("tumblr.com");
		}
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		public override Uri EditUri(Uri uri)
		{
			//If blog post will throw exception but gets caught when downloading
			return new Uri(RemoveQuery(uri).ToString().Replace("/post/", "/image/"));
		}
		protected override Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants().Any(x => x.GetAttributeValue("id", null) == "safemode_actions_display"))
			{
				return Task.FromResult(new ScrapeResult(Enumerable.Empty<string>(), "this tumblr post is locked behind safemode"));
			}

			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return Task.FromResult(new ScrapeResult(images.Select(x => x.GetAttributeValue("content", null)), null));
		}
	}
}
