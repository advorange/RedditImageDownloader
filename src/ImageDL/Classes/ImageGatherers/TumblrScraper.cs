using HtmlAgilityPack;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class TumblrScraper : IWebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@".+\.(tumblr)\..+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"^(?!.*(media\.tumblr)).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public bool IsFromDomain(Uri uri)
		{
			return _DomainRegex.IsMatch(uri.Host);
		}
		public bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		public string EditUri(Uri uri)
		{
			//If blog post will throw exception but gets caught when downloading
			return uri.ToString().Replace("/post/", "/image/");
		}
		public (IEnumerable<string> Uris, string error) Scrape(HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants().Any(x => x.GetAttributeValue("id", null) == "safemode_actions_display"))
			{
				return (Enumerable.Empty<string>(), "this tumblr post is locked behind safemode");
			}

			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return (images.Select(x => x.GetAttributeValue("content", null)), null);
		}
	}
}
