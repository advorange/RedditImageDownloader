using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageScraping
{
	/// <summary>
	/// Scrapes images from imgur.com.
	/// </summary>
	public sealed class ImgurScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"(\/a\/|\/gallery\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <inheritdoc />
		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("imgur.com");
		}
		/// <inheritdoc />
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		/// <inheritdoc />
		public override Uri EditUri(Uri uri)
		{
			var u = RemoveQuery(uri).ToString();
			var code = u.Split('/').Last();
			//If an album or gallery, we can use grid view to get all of the images
			if (RequiresScraping(uri))
			{
				//Albums always have a code that's 5 letters long
				if (code.Length == 5)
				{
					u = $"{u.Replace("/gallery/", "/a/")}?grid";
				}
				//Otherwise it's not an album
				else
				{
					u = $"http://i.imgur.com/{code}.png";
				}
			}
			else if (String.IsNullOrWhiteSpace(Path.GetExtension(u)))
			{
				u = $"https://i.imgur.com/{code}.png";
			}
			return new Uri(u.Replace("_d", "")); //Some thumbnail thing
		}
		/// <inheritdoc />
		protected override Task<ScrapeResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			//Assumes a correctly created album grid url
			var script = doc.DocumentNode.Descendants("script");
			//The hashes are stored in Json in a script that calls window.runSlots
			var wanted = script.Single(x => x.InnerText.Contains("window.runSlots"));

			//Don't bother actually creating any objects from the Json when we only care about the hashes
			var search = "\"hash\":\"";
			var text = wanted.InnerText;
			var hashes = new List<string>();
			for (int index = text.IndexOf(search); index > 0; index = text.IndexOf(search))
			{
				text = text.Substring(index + search.Length);
				hashes.Add(text.Substring(0, text.IndexOf('"')));
			}
			var uris = hashes.Where(x => x.Length == 7).Distinct().Select(x => new Uri($"https://i.imgur.com/{x}.png"));
			return Task.FromResult(new ScrapeResult(uri, false, this, uris, null));
		}
	}
}