using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageScraping
{
	/// <summary>
	/// Scrapes images from a website.
	/// </summary>
	public abstract class WebsiteScraper
	{
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
		/// <param name="client"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public async Task<ScrapeResult> ScrapeAsync(ImageDownloaderClient client, Uri uri)
		{
			HttpResponseMessage resp = null;
			Stream s = null;
			try
			{
				resp = await client.SendWithRefererAsync(uri, HttpMethod.Get).CAF();
				s = await resp.Content.ReadAsStreamAsync().CAF();

				var doc = new HtmlDocument();
				doc.Load(s);

				return await ProtectedScrapeAsync(client, uri, doc).CAF();
			}
			catch (WebException e)
			{
				e.Write();
				return new ScrapeResult(uri, false, this, Enumerable.Empty<Uri>(), e.Message);
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
		/// <param name="client"></param>
		/// <param name="uri"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		protected abstract Task<ScrapeResult> ProtectedScrapeAsync(ImageDownloaderClient client, Uri uri, HtmlDocument doc);
		/// <summary>
		/// Converts the strings to uris, edits them, and then returns the non null values.
		/// </summary>
		/// <param name="uris"></param>
		/// <returns></returns>
		protected IEnumerable<Uri> Convert(IEnumerable<string> uris)
		{
			return uris.Select(x => EditUri(new Uri(x))).Where(x => x != null);
		}
	}
}
