using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class DanbooruScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"(\/posts\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("danbooru.donmai.us");
		}
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		public override Uri EditUri(Uri uri)
		{
			return RemoveQuery(uri);
		}
		protected override Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
		{
			string relativeLocation;

			//Means the 
			var a = doc.DocumentNode.Descendants("a");
			var imageResize = a.SingleOrDefault(x => x.GetAttributeValue("id", null) == "image-resize-link");
			if (imageResize is HtmlNode node)
			{
				var link = "http://danbooru.donmai.us" + node.GetAttributeValue("href", null);
				return Task.FromResult(new ScrapeResult(new[] { link }, null));
			}

			var img = doc.DocumentNode.Descendants("img");
			var image = img.SingleOrDefault(x => x.GetAttributeValue("id", null) == "image");
		}
	}
}
