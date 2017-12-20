using HtmlAgilityPack;
using System;
using System.Linq;

namespace ImageDL.Utilities.Scraping
{
	public static class InstagramScraping
	{
		public static Uri[] ScrapeImages(Uri uri)
		{
			try
			{
				var doc = new HtmlWeb().Load(uri);
				var meta = doc.DocumentNode.Descendants("meta");
				var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
				var content = images.Select(x => x.GetAttributeValue("content", null));
				return content.Where(x => !String.IsNullOrWhiteSpace(x))
					.Select(x => x.StartsWith("https:") ? x : "https:" + x)
					.Select(x => new Uri(x))
					.ToArray();
			}
			catch (Exception e)
			{
				e.WriteException();
				return new Uri[0];
			}
		}
	}
}
