using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ImageDL.Classes.ImageScraping
{
	/*
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
		protected override async Task<ImagesResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants("p").Any(x => x.HasClass("title") && x.InnerText.Contains("R-18")))
			{
				return new ImagesResult(uri, false, this, Enumerable.Empty<Uri>(), "this pixiv post is locked behind the R-18 filter");
			}

			var mode = HttpUtility.ParseQueryString(uri.Query)["mode"];
			switch (mode)
			{
				case "medium": //Shouldn't reach this point since the uri will be edited to mode=manga
					return await ScrapeMediumAsync(client, uri, doc).CAF();
				case "manga":
					return ScrapeManga(uri, doc);
				default:
					throw new InvalidOperationException($"Unknown mode supplied: {mode}.");
			}
		}
		/// <summary>
		/// Scrapes images from a pixiv uri with mode=medium (not recommended).
		/// Has to keep changing the uri until an error occurs then can assume that's where the images stop.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="uri"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		private async Task<ImagesResult> ScrapeMediumAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			var imageContainer = doc.DocumentNode.Descendants("div")
				.Where(x => x.HasClass("img-container") && !x.InnerText.CaseInsContains("doesn't support"));
			if (!imageContainer.Any())
			{
				return new ImagesResult(uri, true, this, new[] { uri }, $"{uri} is animated content (gif/video).");
			}
			var image = imageContainer.SelectMany(x => x.Descendants("img")).SingleOrDefault();
			var iteratedUri = EditUri(new Uri(image.GetAttributeValue("src", null)));

			var validUris = new List<string>();
			var index = 0;
			while (true)
			{
				try
				{
					using (var resp = await client.SendWithRefererAsync(iteratedUri, HttpMethod.Head).CAF())
					{
						if (resp.StatusCode != HttpStatusCode.OK)
						{
							break;
						}
					}

					validUris.Add(iteratedUri.ToString());
					iteratedUri = new Uri(_FindP.Replace(iteratedUri.ToString(), $"_p{++index}_"));
				}
				catch (WebException)
				{
					break;
				}
			}
			return new ImagesResult(uri, false, this, Convert(validUris), null);
		}
		/// <summary>
		/// Scrapes images from a pixiv uri with mode=manga. Able to get all the image uris right away.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		private ImagesResult ScrapeManga(Uri uri, HtmlDocument doc)
		{
			var images = doc.DocumentNode.Descendants("img");
			var mangaImages = images.Where(x => x.GetAttributeValue("data-filter", null) == "manga-image");
			return new ImagesResult(uri, false, this, Convert(mangaImages.Select(x => x.GetAttributeValue("data-src", null))), null);
		}
	}*/
}