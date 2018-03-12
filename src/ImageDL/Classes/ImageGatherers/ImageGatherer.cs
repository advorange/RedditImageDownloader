using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageGatherers
{
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public class ImageGatherer
	{
		protected static List<Regex> AnimatedContentDomains = new List<Regex>
		{
			new Regex(@"\.youtu\.be", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.youtube\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.gfycat\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.streamable\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.v\.redd\.it", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.vimeo\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.dailymotion\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.twitch\.tv", RegexOptions.Compiled | RegexOptions.IgnoreCase),
			new Regex(@"\.liveleak\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase),
		};

		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// The edited <see cref="Uri"/> that will be scraped.
		/// </summary>
		public readonly Uri EditedUri;
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
		public ImmutableArray<Uri> ImageUris { get; private set; }
		/// <summary>
		/// Any errors which have occurred during getting <see cref="ImageUris"/>.
		/// </summary>
		public string Error { get; private set; }

		private ImageGatherer(IEnumerable<WebsiteScraper> scrapers, Uri uri)
		{
			OriginalUri = uri;
			Scraper = scrapers.SingleOrDefault(x => x.IsFromWebsite(OriginalUri));
			EditedUri = Scraper == null ? OriginalUri : Scraper.EditUri(OriginalUri);
			IsAnimated = AnimatedContentDomains.Any(x => x.IsMatch(OriginalUri.ToString()));
		}

		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="ImageGatherer"/> which contains any images gathered and any errors which occurred.</returns>
		public static async Task<ImageGatherer> CreateGatherer(IEnumerable<WebsiteScraper> scrapers, Uri uri)
		{
			var g = new ImageGatherer(scrapers, uri);
			if (!g.IsAnimated && g.Scraper != null && g.Scraper.RequiresScraping(uri))
			{
				var response = await g.Scraper.ScrapeAsync(g.EditedUri).ConfigureAwait(false);
				g.ImageUris = response.Uris.Select(x => g.Scraper.EditUri(x)).Where(x => x != null).ToImmutableArray();
				g.Error = response.Error;
			}
			else
			{
				g.ImageUris = new[] { g.EditedUri }.ToImmutableArray();
			}
			return g;
		}
	}
}