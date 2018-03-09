using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class InstagramScraper : WebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@"\.(instagram)\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/p\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public InstagramScraper() : base(true) { }

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
			return uri;
		}
		protected override Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
		{
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return Task.FromResult(new ScrapeResult(images.Select(x => x.GetAttributeValue("content", null)), null));
		}
	}
}
