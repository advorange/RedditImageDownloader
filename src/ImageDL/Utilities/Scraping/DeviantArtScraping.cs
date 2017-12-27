using HtmlAgilityPack;
using System;
using System.Linq;

namespace ImageDL.Utilities.Scraping
{
	public static class DeviantArtScraping
	{
		/// <summary>
		/// Scrapes images from Deviant Art.
		/// </summary>
		/// <param name="uri">The deviant art page to scrape.</param>
		/// <returns></returns>
		public static Uri[] ScrapeImages(Uri uri)
		{
			try
			{
				var doc = new HtmlWeb().Load(uri);
				var images = doc.DocumentNode.Descendants("img");
				var deviations = images.Where(x =>
				{
					return x.GetAttributeValue("data-embed-type", null) == "deviation"
						&& x.GetAttributeValue("dev-content-full", null) != null;
				});
				var src = deviations.Select(x => x.GetAttributeValue("src", null));
				return src.Where(x => !String.IsNullOrWhiteSpace(x))
					.Select(x => UriUtils.MakeUri(x))
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
