using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
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
			throw new NotImplementedException();
		}
	}
}
