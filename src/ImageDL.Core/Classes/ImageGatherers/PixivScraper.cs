using AdvorangesUtils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ImageDL.Classes.ImageGatherers
{
	/// <summary>
	/// Scrapes images from pixiv.net.
	/// </summary>
	public sealed class PixivScraper : WebsiteScraper
	{
		//Remove size arguments from an image (i.e. /c/600x600/ is saying to have a 600x600 image which we don't want)
		private static Regex _RemoveC = new Regex(@"\/c\/(\d*?)x(\d*?)\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		//Find _p0_ in a url so we can replace it with an incremented version to get the next image in the post
		private static Regex _FindP = new Regex(@"_p(\d*?)_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		//Replace the mode with manga so all the images can easily be gotten
		private static Regex _Mode = new Regex(@"mode=.*?&", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <inheritdoc />
		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("pixiv.net");
		}
		/// <inheritdoc />
		public override bool RequiresScraping(Uri uri)
		{
			return true;
		}
		/// <inheritdoc />
		public override Uri EditUri(Uri uri)
		{
			return new Uri(_Mode.Replace(_RemoveC.Replace(uri.ToString(), "/"), "mode=manga&"));
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants("p").Any(x => x.HasClass("title") && x.InnerText.Contains("R-18")))
			{
				return new ScrapeResult(Enumerable.Empty<string>(), "this pixiv post is locked behind the R-18 filter");
			}

			var mode = HttpUtility.ParseQueryString(uri.Query)["mode"];
			switch (mode)
			{
				case "medium": //Shouldn't reach this point since the uri will be edited to mode=manga
					return await ScrapeMediumAsync(doc).CAF();
				case "manga":
					return ScrapeManga(doc);
				default:
					throw new InvalidOperationException($"Unknown mode supplied: {mode}.");
			}
		}
		/// <summary>
		/// Scrapes images from a pixiv uri with mode=medium (not recommended).
		/// Has to keep changing the uri until an error occurs then can assume that's where the images stop.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		private async Task<ScrapeResult> ScrapeMediumAsync(HtmlDocument doc)
		{
			var imageContainer = doc.DocumentNode.Descendants("div").Where(x => x.HasClass("img-container"));
			var image = imageContainer.SelectMany(x => x.Descendants("img")).SingleOrDefault();
			var uri = EditUri(new Uri(image.GetAttributeValue("src", null)));

			var validUris = new List<string>();
			var index = 0;
			while (true)
			{
				try
				{
					var req = CreateWebRequest(uri);
					req.Method = WebRequestMethods.Http.Head;

					using (var resp = (HttpWebResponse)(await req.GetResponseAsync().CAF()))
					{
						if (resp.StatusCode != HttpStatusCode.OK)
						{
							break;
						}
					}

					validUris.Add(uri.ToString());
					uri = new Uri(_FindP.Replace(uri.ToString(), $"_p{++index}_"));
				}
				catch (WebException)
				{
					break;
				}
			}
			return new ScrapeResult(validUris, null);
		}
		/// <summary>
		/// Scrapes images from a pixiv uri with mode=manga. Able to get all the image uris right away.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		private ScrapeResult ScrapeManga(HtmlDocument doc)
		{
			var images = doc.DocumentNode.Descendants("img");
			var mangaImages = images.Where(x => x.GetAttributeValue("data-filter", null) == "manga-image");
			return new ScrapeResult(mangaImages.Select(x => x.GetAttributeValue("data-src", null)), null);
		}
	}
}
