using HtmlAgilityPack;
using System;
using System.Linq;

namespace RedditImageDownloader
{
	public static class ImgurScraper
	{
		public static string[] Scrape(string url)
		{
			try
			{
				var doc = new HtmlWeb().Load(url);
				var images = doc.DocumentNode.Descendants("img");
				//Not sure if this is a foolproof way to only get the wanted images
				var mainImages = images.Where(x => x.Attributes["itemprop"] != null);
				var src = mainImages.Select(x => x.GetAttributeValue("src", null));
				return src
					.Where(x => !String.IsNullOrWhiteSpace(x))
					.Select(x => x.StartsWith("https:") ? x : "https:" + x)
					.ToArray();
			}
			catch (Exception e)
			{
				HelperActions.WriteException(e);
				return new string[0];
			}
		}
	}
}
