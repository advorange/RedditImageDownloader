using HtmlAgilityPack;
using System;
using System.Linq;

namespace MassDownloadImages.HelperClasses
{
	/// <summary>
	/// Scrapes data from imgur.
	/// </summary>
	public static class ImgurScraping
	{
		/// <summary>
		/// Scrapes images from imgur.
		/// </summary>
		/// <param name="uri">The imgur page to scrape.</param>
		/// <returns></returns>
		public static Uri[] ScrapeImages(Uri uri)
		{
			try
			{
				var doc = new HtmlWeb().Load(uri);
				var images = doc.DocumentNode.Descendants("img");
				//Not sure if this is a foolproof way to only get the wanted images
				var mainImages = images.Where(x => x.Attributes["itemprop"] != null);
				var src = mainImages.Select(x => x.GetAttributeValue("src", null));
				return src.Where(x => !String.IsNullOrWhiteSpace(x))
					.Select(x => x.StartsWith("https:") ? x : "https:" + x)
					.Select(x => new Uri(x))
					.ToArray();
			}
			catch (Exception e)
			{
				HelperActions.WriteException(e);
				return new Uri[0];
			}
		}
	}
}
