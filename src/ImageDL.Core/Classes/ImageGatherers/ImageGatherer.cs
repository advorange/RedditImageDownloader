using AdvorangesUtils;
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
		/// <summary>
		/// Regex for checking if a uri leads to animated content (video, gif, etc).
		/// </summary>
		public static List<Regex> AnimatedContentDomains = new List<Regex>
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
		public Uri OriginalUri { get; private set; }
		/// <summary>
		/// Indicates whether or not the link leads to a video site.
		/// </summary>
		public bool IsAnimated { get; private set; }
		/// <summary>
		/// The scraper used for the domain.
		/// </summary>
		public WebsiteScraper Scraper { get; private set; }
		/// <summary>
		/// The images to download.
		/// </summary>
		public ImmutableArray<Uri> GatheredUris { get; private set; }
		/// <summary>
		/// Any errors which have occurred during getting <see cref="GatheredUris"/>.
		/// </summary>
		public string Error { get; private set; }

		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="scrapers">The scrapers to use for gathering images from webpages.</param>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="ImageGatherer"/> which contains any images gathered and any errors which occurred.</returns>
		public static async Task<ImageGatherer> CreateGathererAsync(IEnumerable<WebsiteScraper> scrapers, Uri uri)
		{
			var scraper = scrapers.SingleOrDefault(x => x.IsFromWebsite(uri));
			var g = new ImageGatherer
			{
				OriginalUri = uri,
				Scraper = scraper,
				IsAnimated = AnimatedContentDomains.Any(x => x.IsMatch(uri.ToString())),
			};

			var editedUri = scraper == null ? uri : scraper.EditUri(uri);
			//If the link goes directly to an image, just use that
			if (editedUri.ToString().IsImagePath())
			{
				g.GatheredUris = new[] { editedUri }.ToImmutableArray();
			}
			//If the link is animated, return nothing and give an error
			else if (g.IsAnimated)
			{
				g.GatheredUris = new[] { editedUri }.ToImmutableArray();
				g.Error = $"{editedUri} is animated content (gif/video).";
			}
			//If the scraper isn't null and the uri requires scraping, scrape it
			else if (g.Scraper != null && g.Scraper.RequiresScraping(uri))
			{
				var response = await g.Scraper.ScrapeAsync(g.OriginalUri).CAF();
				g.GatheredUris = response.Uris.Select(x => g.Scraper.EditUri(x)).Where(x => x != null).ToImmutableArray();
				g.Error = response.Error;
			}
			//Otherwise, just return the uri and hope for the best.
			else
			{
				g.GatheredUris = new[] { editedUri }.ToImmutableArray();
			}
			return g;
		}
	}
}