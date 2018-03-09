using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class ImgurScraper : WebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@"\.(imgur)\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/a\/|\/gallery\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public ImgurScraper() : base(true) { }

		public override bool IsFromWebsite(Uri uri)
		{
			return _DomainRegex.IsMatch(uri.Host);
		}
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		protected override Uri ProtectedEditUri(Uri uri)
		{
			var u = uri.ToString();
			u = String.IsNullOrEmpty(Path.GetExtension(u)) ? $"https://i.imgur.com/{u.Substring(u.LastIndexOf('/') + 1)}.png" : u;
			return new Uri(u.Replace("_d", "")); //Some thumbnail thing
		}
		protected override Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
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
			return Task.FromResult(new ScrapeResult(images.Select(x => $"https://i.imgur.com/{x}.png"), null));
		}
	}
}
