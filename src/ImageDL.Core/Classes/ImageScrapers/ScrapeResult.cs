using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImageDL.Classes.ImageScrapers
{
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public class ScrapeResult
	{
		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// Indicates whether or not the link leads to a video site.
		/// </summary>
		public readonly bool IsAnimated;
		/// <summary>
		/// The scraper used for the domain.
		/// </summary>
		public readonly WebsiteScraper Scraper;
		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUris;
		/// <summary>
		/// Any errors which have occurred during getting <see cref="ImageUris"/>.
		/// </summary>
		public readonly string Error;

		/// <summary>
		/// Creates an instance of <see cref="ScrapeResult"/>.
		/// </summary>
		/// <param name="originalUri"></param>
		/// <param name="isAnimated"></param>
		/// <param name="scraper"></param>
		/// <param name="gatheredUris"></param>
		/// <param name="error"></param>
		public ScrapeResult(Uri originalUri, bool isAnimated, WebsiteScraper scraper, IEnumerable<Uri> gatheredUris, string error)
		{
			OriginalUri = originalUri;
			IsAnimated = isAnimated;
			Scraper = scraper;
			ImageUris = gatheredUris.ToImmutableArray();
			Error = error;
		}
	}
}
