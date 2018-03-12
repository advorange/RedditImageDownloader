using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	public sealed class DeviantArtScraper : WebsiteScraper
	{
		private static Regex _DomainRegex = new Regex(@"\.(deviantart)\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _ScrapeRegex = new Regex(@"(\/art\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
			//18+ filter (shouldn't be reached since the cookies are set)
			if (doc.DocumentNode.Descendants("div").Any(x => x.HasClass("dev-content-mature")))
			{
				//Scuffed way to get the images even when the 18+ filter is on
				var img = doc.DocumentNode.Descendants("img")
				.Where(x =>
				{
					var attrs = x.Attributes;
					return attrs["width"] != null && attrs["height"] != null && attrs["alt"] != null && attrs["src"] != null && attrs["srcset"] != null && attrs["sizes"] != null;
				})
				.Select(x => x.GetAttributeValue("srcset", null))
				.Select(x =>
				{
					var w = x.LastIndexOf("w,") + 2; //W for w,
					var s = x.LastIndexOf(' '); //S for space
					return w < 0 || s < 0 || w > s ? null : x.Substring(w, s - w);
				});

				return Task.FromResult(new ScrapeResult(img, img.Any() ? null : "this deviantart post is locked behind mature content"));
			}

			var images = doc.DocumentNode.Descendants("img");
			var deviations = images.Where(x => x.GetAttributeValue("data-embed-type", null) == "deviation");
			return Task.FromResult(new ScrapeResult(deviations.Select(x => x.GetAttributeValue("src", null)), null));
		}
	}
}
