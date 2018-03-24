using AdvorangesUtils;
using HtmlAgilityPack;
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
	/// Scrapes images from a website.
	/// </summary>
	public abstract class WebsiteScraper
	{
		/// <summary>
		/// Creates a web request and sets some properties to make it look more human.
		/// </summary>
		/// <param name="uri">The site to navigate to.</param>
		/// <returns>A webrequest to <paramref name="uri"/>.</returns>
		public static HttpWebRequest CreateWebRequest(Uri uri)
		{
			var req = (HttpWebRequest)WebRequest.Create(uri);

			req.Headers["Accept-Language"] = "en-US"; //Make sure we get English results
			req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
			req.Referer = uri.ToString(); //Pixiv requires a referer that is a valid link on Pixiv. You can pass in the same link.

			req.Timeout = 5000;
			req.ReadWriteTimeout = 5000;
			req.AllowAutoRedirect = true; //So Imgur can redirect to correct webpages

			req.Credentials = CredentialCache.DefaultCredentials;
			req.Proxy = new WebProxy(); //One of my computers throws an exception if the proxy is null

			req.CookieContainer = new CookieContainer();
			req.CookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter

			return req;
		}
		/// <summary>
		/// Removes query parameters from a url.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public Uri RemoveQuery(Uri uri)
		{
			var u = uri.ToString();
			return u.CaseInsIndexOf("?", out var index) ? new Uri(u.Substring(0, index)) : uri;
		}
		/// <summary>
		/// Attempts to get images from <paramref name="uri"/>. Will not edit <paramref name="uri"/> at all. 
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public async Task<ScrapeResult> ScrapeAsync(Uri uri)
		{
			WebResponse resp = null;
			Stream s = null;
			try
			{
				var doc = new HtmlDocument();
				doc.Load(s = (resp = await CreateWebRequest(uri).GetResponseAsync().CAF()).GetResponseStream());

				return await ProtectedScrapeAsync(uri, doc).CAF();
			}
			catch (WebException e)
			{
				e.Write();
				return new ScrapeResult(Enumerable.Empty<string>(), e.Message);
			}
			finally
			{
				resp?.Dispose();
				s?.Dispose();
			}
		}
		/// <summary>
		/// Determines if the uri can be scraped with this scraper.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public abstract bool IsFromWebsite(Uri uri);
		/// <summary>
		/// Determines if the uri requires scraping.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public abstract bool RequiresScraping(Uri uri);
		/// <summary>
		/// Edits the uri to remove unnecessary parts.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public abstract Uri EditUri(Uri uri);
		/// <summary>
		/// Scrapes the wanted information from formatted HTML.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		protected abstract Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc);

		/// <summary>
		/// The results from scraping a website.
		/// </summary>
		public sealed class ScrapeResult
		{
			/// <summary>
			/// The gotten image uris from scraping the website.
			/// </summary>
			public readonly ImmutableArray<Uri> Uris;
			/// <summary>
			/// Any error gotten when attemping to scrape the website.
			/// </summary>
			public readonly string Error;

			/// <summary>
			/// Converts the strings to uris and sets the values.
			/// </summary>
			/// <param name="uris">The scraped uris.</param>
			/// <param name="error">The gotten error.</param>
			public ScrapeResult(IEnumerable<string> uris, string error)
			{
				Uris = uris.Select(x => new Uri(x)).ToImmutableArray();
				Error = error;
			}
		}
	}
}
