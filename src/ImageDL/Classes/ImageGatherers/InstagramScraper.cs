using HtmlAgilityPack;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class InstagramScraper : IWebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@".+\.(instagram)\..+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/p\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
			return uri.ToString();
		}
		public (IEnumerable<string> Uris, string error) Scrape(HtmlDocument doc)
		{
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return (images.Select(x => x.GetAttributeValue("content", null)), null);
		}
	}
}
