using HtmlAgilityPack;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public class ImageGatherer
	{
		protected static List<string> AnimatedContentDomains = new List<string>
		{
			"youtu.be",
			"youtube",
			"gfycat",
			"streamable",
			"v.redd.it",
			"vimeo",
			"dailymotion",
			"twitch",
			"liveleak",
		};
		protected static List<IWebsiteScraper> Scrapers = new List<IWebsiteScraper>
		{
			new ImgurScraper(),
			new TumblrScraper(),
			new DeviantArtScraper(),
			new InstagramScraper(),
		};

		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// Similar to <see cref="OriginalUri"/> except removes optional arguments and fixes some weird things sites do with their urls.
		/// </summary>
		public readonly Uri EditedUri;
		/// <summary>
		/// Indicates whether or not the link leads to a video site.
		/// </summary>
		public readonly bool IsVideo;
		/// <summary>
		/// The scraper to use for the specified website.
		/// </summary>
		public readonly IWebsiteScraper WebsiteScraper;
		/// <summary>
		/// The images to download.
		/// </summary>
		public ImmutableList<Uri> ImageUris { get; private set; }
		/// <summary>
		/// Any errors which have occurred during getting <see cref="ImageUris"/>.
		/// </summary>
		public string Error { get; private set; }

		private ImageGatherer(Uri uri)
		{
			OriginalUri = uri;
			WebsiteScraper = Scrapers.SingleOrDefault(x => x.IsFromDomain(uri));
			EditedUri = EditUri(WebsiteScraper, uri.ToString());
			IsVideo = AnimatedContentDomains.Any(x => uri.ToString().CaseInsContains(x));
		}

		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="ImageGatherer"/> which contains any images gathered and any errors which occurred.</returns>
		public static async Task<ImageGatherer> CreateGatherer(Uri uri)
		{
			var g = new ImageGatherer(uri);
			if (!g.IsVideo && g.WebsiteScraper.RequiresScraping(uri))
			{
				var response = await ScrapeImages(g.WebsiteScraper, g.OriginalUri).ConfigureAwait(false);
				g.ImageUris = response.Uris.Select(x => EditUri(g.WebsiteScraper, x)).Where(x => x != null).ToImmutableList();
				g.Error = response.Error;
			}
			else
			{
				g.ImageUris = new[] { g.EditedUri }.ToImmutableList();
			}
			return g;
		}
		/// <summary>
		/// Edits the provided uri by removing optional arguments, adding in http or https, and then editing the uri with the passed in scraper.
		/// </summary>
		/// <param name="scraper"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		private static Uri EditUri(IWebsiteScraper scraper, string uri)
		{
			//Remove all optional arguments
			if (uri.CaseInsIndexOf("?", out var pos))
			{
				uri = uri.Substring(0, pos);
			}
			//Http/Https
			if (!uri.CaseInsStartsWith("http://") && !uri.CaseInsStartsWith("https://"))
			{
				uri = uri.Contains("//") ? uri.Substring(uri.IndexOf("//") + 2) : uri;
				uri = "https://" + uri;
			}
			return Uri.TryCreate(uri, UriKind.Absolute, out var result)
				&& Uri.TryCreate(scraper.EditUri(result), UriKind.Absolute, out var edited) ? edited : null;
		}
		/// <summary>
		/// Scrapes images from the 
		/// </summary>
		/// <param name="scraper"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		private static async Task<(IEnumerable<string> Uris, string Error)> ScrapeImages(IWebsiteScraper scraper, Uri uri)
		{
			WebResponse resp = null;
			Stream s = null;
			try
			{
				var req = uri.CreateWebRequest();
				req.CookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter

				var doc = new HtmlDocument();
				doc.Load(s = (resp = await req.GetResponseAsync().ConfigureAwait(false)).GetResponseStream());

				return scraper.Scrape(doc);
			}
			catch (WebException e)
			{
				e.Write();
				return (Enumerable.Empty<string>(), e.Message);
			}
			finally
			{
				resp?.Dispose();
				s?.Dispose();
			}
		}
	}
}
