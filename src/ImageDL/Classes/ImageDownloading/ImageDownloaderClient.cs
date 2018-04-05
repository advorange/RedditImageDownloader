using AdvorangesUtils;
using HtmlAgilityPack;
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
	public class ImageDownloaderClient : HttpClient, IImageDownloaderClient
	{
		/// <inheritdoc />
		public List<Regex> AnimatedContentDomains { get; set; } = new List<Regex>
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
		/// <inheritdoc />
		public List<IImageGatherer> Gatherers { get; set; } = typeof(IImageGatherer).Assembly.DefinedTypes
			.Where(x => x.IsSubclassOf(typeof(IImageGatherer)))
			.Select(x => Activator.CreateInstance(x))
			.Cast<IImageGatherer>()
			.ToList();
		/// <inheritdoc />
		public Dictionary<Type, ApiKey> ApiKeys { get; set; } = new Dictionary<Type, ApiKey>();
		/// <inheritdoc />
		public CookieContainer Cookies { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="ImageDownloaderClient"/>.
		/// </summary>
		public ImageDownloaderClient() : this(new CookieContainer()) { }
		/// <summary>
		/// Creates an instance of <see cref="ImageDownloaderClient"/>.
		/// </summary>
		public ImageDownloaderClient(CookieContainer cookies) : base(GetDefaultClientHandler(cookies))
		{
			Cookies = cookies;
			Timeout = TimeSpan.FromMilliseconds(60000);
			DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36 (compatible; MSIE 4.01; AOL 4.0; Mac_68K) (+https://github.com/advorange/ImageDL)");
			DefaultRequestHeaders.Add("Accept-Language", "en-US"); //Make sure we get English results
		}

		private static HttpClientHandler GetDefaultClientHandler(CookieContainer cookies)
		{
			cookies.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter

			return new HttpClientHandler
			{
				AllowAutoRedirect = true, //So Imgur can redirect to correct webpages
				Credentials = CredentialCache.DefaultCredentials,
				Proxy = new WebProxy(), //One of my computers throws an exception if the proxy is null
				CookieContainer = cookies,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};
		}

		/// <inheritdoc />
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
		/// <inheritdoc />
		public async Task<ClientResult<string>> GetText(Uri uri, TimeSpan wait = default, int tries = 3)
		{
			wait = wait == default ? TimeSpan.FromSeconds(2) : wait;
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
						ConsoleUtils.WriteLine($"Rate limited; retrying next at: {nextRetry.ToLongTimeString()}");
						continue;
					}

					return new ClientResult<string>(await resp.Content.ReadAsStringAsync().CAF(), resp.StatusCode, resp.IsSuccessStatusCode);
				}
			}
			throw new HttpRequestException($"Unable to get the requested webpage after {tries} tries.");
		}
		/// <inheritdoc />
		public async Task<ClientResult<HtmlDocument>> GetHtml(Uri uri, TimeSpan wait = default, int tries = 3)
		{
			var result = await GetText(uri).CAF();
			if (result.IsSuccess)
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(result.Value);
				return new ClientResult<HtmlDocument>(doc, result.StatusCode, result.IsSuccess);
			}
			else
			{
				return new ClientResult<HtmlDocument>(null, result.StatusCode, result.IsSuccess);
			}
		}
		/// <inheritdoc />
		public async Task<GatheredImagesResponse> GetImagesAsync(Uri uri)
		{
			if (uri.ToString().IsImagePath())
			{
				return GatheredImagesResponse.FromImage(uri);
			}
			//If the link is animated, return nothing and give an error
			else if (AnimatedContentDomains.Any(x => x.IsMatch(uri.ToString())))
			{
				return GatheredImagesResponse.FromAnimated(uri);
			}
			//If there is a gatherer, use it
			else if (Gatherers.SingleOrDefault(x => x.IsFromWebsite(uri)) is IImageGatherer gatherer)
			{
				try
				{
					return await gatherer.GetImagesAsync(this, uri).CAF();
				}
				catch (Exception e)
				{
					return GatheredImagesResponse.FromException(uri, e);
				}
			}
			//Otherwise, just return the uri and hope for the best
			else
			{
				return GatheredImagesResponse.FromUnknown(uri);
			}
		}
		/// <summary>
		/// Removes query parameters from a url.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static Uri RemoveQuery(Uri uri)
		{
			var u = uri.ToString();
			return u.CaseInsIndexOf("?", out var index) ? new Uri(u.Substring(0, index)) : uri;
		}
	}

	/// <summary>
	/// Result of getting the text from a webpage.
	/// </summary>
	public struct ClientResult<T>
	{
		/// <summary>
		/// The value of the request.
		/// </summary>
		public readonly T Value;
		/// <summary>
		/// The http status code for the request.
		/// </summary>
		public readonly HttpStatusCode StatusCode;
		/// <summary>
		/// Whether or not the request was successful.
		/// </summary>
		public readonly bool IsSuccess;

		/// <summary>
		/// Creates an instance of <see cref="ClientResult{T}"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="statusCode"></param>
		/// <param name="isSuccess"></param>
		public ClientResult(T value, HttpStatusCode statusCode, bool isSuccess)
		{
			Value = value;
			StatusCode = statusCode;
			IsSuccess = isSuccess;
		}
	}
}