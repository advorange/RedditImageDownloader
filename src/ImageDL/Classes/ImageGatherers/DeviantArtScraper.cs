using HtmlAgilityPack;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class DeviantArtScraper : IWebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@".+\.(deviantart)\..+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/art\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
			//18+ filter (shouldn't be reached since the cookies are set)
			if (doc.DocumentNode.Descendants("div").Any(x => x.HasClass("dev-content-mature")))
			{
				//Scuffed way to get the images even when the 18+ filter is on
				var img = doc.DocumentNode.Descendants("img")
				.Where(x =>
				{
					var attrs = x.Attributes;
					return attrs["width"] != null && attrs["height"] != null && attrs["alt"] != null && attrs["src"] != null && attrs["srcset"] != null && attrs["sizes"] != null;
				})
				.Select(x => x.GetAttributeValue("srcset", null))
				.Select(x =>
				{
					var w = x.LastIndexOf("w,") + 2; //W for w,
					var s = x.LastIndexOf(' '); //S for space
					return w < 0 || s < 0 || w > s ? null : x.Substring(w, s - w);
				});

				return img.Any() ? (img, null) : (Enumerable.Empty<string>(), "this deviantart post is locked behind mature content");
			}

			var images = doc.DocumentNode.Descendants("img");
			var deviations = images.Where(x => x.GetAttributeValue("data-embed-type", null) == "deviation");
			return (deviations.Select(x => x.GetAttributeValue("src", null)), null);
		}
	}
}
