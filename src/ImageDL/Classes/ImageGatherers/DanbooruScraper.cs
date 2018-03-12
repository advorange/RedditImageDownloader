using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class DanbooruScraper : WebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@"(danbooru)\.(donmai)\.us", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/posts\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override bool IsFromWebsite(Uri uri)
		{
			return _DomainRegex.IsMatch(uri.Host);
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
			throw new NotImplementedException();
		}
	}
}
