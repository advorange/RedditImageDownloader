using HtmlAgilityPack;
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
	public abstract class WebsiteScraper
	{
		public readonly bool RemoveOptionalArguments;

		public WebsiteScraper(bool removeOptionalArguments)
		{
			RemoveOptionalArguments = removeOptionalArguments;
		}

		/// <summary>
		/// Can remove query parameters if that was set as true in the constructor, and calls the custom implementation after.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public Uri EditUri(Uri uri)
		{
			var u = uri.ToString();
			if (RemoveOptionalArguments && u.CaseInsIndexOf("?", out var pos))
			{
				u = u.Substring(0, pos);
			}
			return ProtectedEditUri(new Uri(u));
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
				doc.Load(s = (resp = await uri.CreateWebRequest().GetResponseAsync().ConfigureAwait(false)).GetResponseStream());

				return await ProtectedScrapeAsync(uri, doc).ConfigureAwait(false);
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

		public abstract bool IsFromWebsite(Uri uri);
		public abstract bool RequiresScraping(Uri uri);
		protected abstract Uri ProtectedEditUri(Uri uri);
		protected abstract Task<ScrapeResult> ProtectedScrapeAsync(Uri uri, HtmlDocument doc);

		public sealed class ScrapeResult
		{
			public readonly ImmutableArray<Uri> Uris;
			public readonly string Error;

			public ScrapeResult(IEnumerable<string> uris, string error)
			{
				Uris = uris.Select(x => new Uri(x)).ToImmutableArray();
				Error = error;
			}
		}
	}
}
