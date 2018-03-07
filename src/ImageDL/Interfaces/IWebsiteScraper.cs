using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace ImageDL.Interfaces
{
	public interface IWebsiteScraper
	{
		bool IsFromDomain(Uri uri);
		bool RequiresScraping(Uri uri);
		string EditUri(Uri uri);
		(IEnumerable<string> Uris, string error) Scrape(HtmlDocument doc);
	}
}
