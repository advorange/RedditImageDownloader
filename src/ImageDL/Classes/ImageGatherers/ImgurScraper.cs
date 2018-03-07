using HtmlAgilityPack;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class ImgurScraper : IWebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@".+\.(imgur)\..+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/a\/|\/gallery\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
			var u = uri.ToString();
			u = String.IsNullOrEmpty(Path.GetExtension(u)) ? $"https://i.imgur.com/{u.Substring(u.LastIndexOf('/') + 1)}.png" : u;
			return u.Replace("_d", ""); //Some thumbnail thing
		}
		public (IEnumerable<string> Uris, string error) Scrape(HtmlDocument doc)
		{
			//Only works on albums with less than 10 images
			//Otherwise not all the images load in as images, but their ids will still be there.
#if false
			var images = doc.DocumentNode.Descendants("img");
			var itemprop = images.Where(x => x.GetAttributeValue("itemprop", null) != null);
			return new ScrapeResponse(itemprop.Select(x => x.GetAttributeValue("src", null)));
#endif
			var div = doc.DocumentNode.Descendants("div");
			var images = div.Where(x => x.GetAttributeValue("itemtype", null) == "http://schema.org/ImageObject")
				.Select(x => x.GetAttributeValue("id", null))
				.Where(x => x != null);
			return (images.Select(x => $"https://i.imgur.com/{x}.png"), null);
		}
	}
}
