﻿using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Client used to scrape images and download images.
	/// </summary>
	public class ImageDownloaderClient : HttpClient
	{
		/// <summary>
		/// Regex for checking if a uri leads to animated content (video, gif, etc).
		/// </summary>
		public List<Regex> AnimatedContentDomains { get; set; }
		/// <summary>
		/// Scrapers for gathering images from websites.
		/// </summary>
		public List<WebsiteScraper> Scrapers { get; set; }
		/// <summary>
		/// The currently used API key. This doesn't lead to any site in specific.
		/// </summary>
		public string APIKey => _APIKey;
		/// <summary>
		/// The last time the API key was updated.
		/// </summary>
		public DateTime APIKeyLastUpdated => _APIKeyLastUpdated;
		/// <summary>
		/// How long until the API key is invalid.
		/// </summary>
		public TimeSpan APIKeyDuration => _APIKeyDuration;

		private string _APIKey;
		private DateTime _APIKeyLastUpdated;
		private TimeSpan _APIKeyDuration;

		/// <summary>
		/// Creates an instance of <see cref="ImageDownloaderClient"/>.
		/// </summary>
		public ImageDownloaderClient() : base(GetDefaultClientHandler())
		{
			AnimatedContentDomains = GetDefaultAnimatedContentDomains();
			Scrapers = GetDefaultScrapers();

			Timeout = TimeSpan.FromMilliseconds(60000);
			DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36 (compatible; MSIE 4.01; AOL 4.0; Mac_68K) (+https://github.com/advorange/ImageDL)");
			DefaultRequestHeaders.Add("Accept-Language", "en-US"); //Make sure we get English results
		}

		private static HttpClientHandler GetDefaultClientHandler()
		{
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter

			return new HttpClientHandler
			{
				AllowAutoRedirect = true, //So Imgur can redirect to correct webpages
				Credentials = CredentialCache.DefaultCredentials,
				Proxy = new WebProxy(), //One of my computers throws an exception if the proxy is null
				CookieContainer = cookieContainer,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};
		}
		private static List<Regex> GetDefaultAnimatedContentDomains()
		{
			return new List<Regex>
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
		}
		private static List<WebsiteScraper> GetDefaultScrapers()
		{
			return typeof(IImageDownloader).Assembly.DefinedTypes
				.Where(x => x.IsSubclassOf(typeof(WebsiteScraper)))
				.Select(x => Activator.CreateInstance(x))
				.Cast<WebsiteScraper>()
				.ToList();
		}

		/// <summary>
		/// Updates the stored API key and the time it needs to reset.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="duration"></param>
		public void UpdateAPIKey(string key, TimeSpan duration)
		{
			_APIKey = key;
			_APIKeyLastUpdated = DateTime.UtcNow;
			_APIKeyDuration = duration;
		}
		/// <summary>
		/// Sends a GET request to get the main text of the link. Waits for the passed in wait time multiplied by 2 for each failure.
		/// Will throw if tries are used up/all errors other than 421 and 429.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="wait">The amount of time to wait between retries. This will be doubled each retry.</param>
		/// <param name="tries"></param>
		/// <returns></returns>
		/// <exception cref="HttpRequestException">If unable to get the request after all retries have been used up.</exception>
		public async Task<string> GetMainTextAndRetryIfRateLimitedAsync(Uri uri, TimeSpan wait, int tries = 3)
		{
			var nextRetry = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
			for (int i = 0; i < tries; ++i)
			{
				var diff = nextRetry - DateTime.UtcNow;
				if (diff.Ticks > 0)
				{
					await Task.Delay(diff).CAF();
				}

				using (var resp = await SendWithRefererAsync(uri, HttpMethod.Get).CAF())
				{
					var code = (int)resp.StatusCode;
					if (code == 421 || code == 429) //Rate limit error codes
					{
						//Wait longer on each failure
						nextRetry = DateTime.UtcNow.Add(TimeSpan.FromTicks(wait.Ticks * (int)Math.Pow(2, i)));
						Console.WriteLine($"Rate limited; retrying next at: {nextRetry.ToLongTimeString()}");
						continue;
					}
					else if (!resp.IsSuccessStatusCode)
					{
						//Just throw at this point since it's most likely irrecoverable
						resp.EnsureSuccessStatusCode();
					}
					return await resp.Content.ReadAsStringAsync().CAF();
				}
			}
			throw new HttpRequestException($"Unable to get the requested text after {tries} tries.");
		}
		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="ScrapeResult"/> which contains any images gathered and any errors which occurred.</returns>
		public async Task<ScrapeResult> ScrapeImagesAsync(Uri uri)
		{
			var scraper = Scrapers.SingleOrDefault(x => x.IsFromWebsite(uri));
			var isAnimated = AnimatedContentDomains.Any(x => x.IsMatch(uri.ToString()));
			var editedUri = scraper == null ? uri : scraper.EditUri(uri);

			//If the link goes directly to an image, just use that
			if (editedUri.ToString().IsImagePath())
			{
				return new ScrapeResult(uri, isAnimated, null, new[] { editedUri }, null);
			}
			//If the link is animated, return nothing and give an error
			else if (isAnimated)
			{
				return new ScrapeResult(uri, isAnimated, null, new[] { editedUri }, $"{editedUri} is animated content (gif/video).");
			}
			//If the scraper isn't null and the uri requires scraping, scrape it
			else if (scraper != null && scraper.RequiresScraping(uri))
			{
				return await scraper.ScrapeAsync(this, uri).CAF();
			}
			//Otherwise, just return the uri and hope for the best.
			else
			{
				return new ScrapeResult(uri, isAnimated, scraper, new[] { editedUri }, null);
			}
		}
		/// <summary>
		/// Sends a request to the uri with the specified method and the referer as itself.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public async Task<HttpResponseMessage> SendWithRefererAsync(Uri uri, HttpMethod method)
		{
			var req = new HttpRequestMessage
			{
				RequestUri = uri,
				Method = method,
			};
			req.Headers.Referrer = uri; //Set self as referer since Pixiv requires a valid Pixiv url as its referer
			return await SendAsync(req).CAF();
		}
	}
}