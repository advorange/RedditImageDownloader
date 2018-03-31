using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageScraping
{
	/// <summary>
	/// Scrapes images from danbooru.donmai.us.
	/// </summary>
	public sealed class DanbooruScraper : WebsiteScraper
	{
		private static Regex _ScrapeRegex = new Regex(@"(\/posts\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <inheritdoc />
		public override bool IsFromWebsite(Uri uri)
		{
			return uri.Host.CaseInsContains("danbooru.donmai.us");
		}
		/// <inheritdoc />
		public override bool RequiresScraping(Uri uri)
		{
			return _ScrapeRegex.IsMatch(uri.ToString());
		}
		/// <inheritdoc />
		public override Uri EditUri(Uri uri)
		{
			return RemoveQuery(uri);
		}
		/// <inheritdoc />
		protected override Task<ScrapeResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc)
		{
			var a = doc.DocumentNode.Descendants("a");
			var imageResize = a.SingleOrDefault(x => x.GetAttributeValue("id", null) == "image-resize-link");
			if (imageResize is HtmlNode imageResizeNode)
			{
				var link = "http://danbooru.donmai.us" + imageResizeNode.GetAttributeValue("href", null);
				return Task.FromResult(new ScrapeResult(uri, false, this, Convert(new[] { link }), null));
			}

			var img = doc.DocumentNode.Descendants("img");
			var image = img.SingleOrDefault(x => x.GetAttributeValue("id", null) == "image");
			if (image is HtmlNode imageNode)
			{
				var link = "http://danbooru.donmain.us" + imageNode.GetAttributeValue("src", null);
				return Task.FromResult(new ScrapeResult(uri, false, this, Convert(new[] { link }), null));
			}

			return Task.FromResult(new ScrapeResult(uri, false, this, Enumerable.Empty<Uri>(), "Unable to gather any images."));
		}
	}
}