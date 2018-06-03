using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using BrotliSharpLib;
using HtmlAgilityPack;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Client used to scrape images and download images.
	/// </summary>
	public class DownloaderClient : HttpClient, IDownloaderClient
	{
		/// <inheritdoc />
		public string UserAgent => "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.170 Safari/537.36 OPR/53.0.2907.68 (+https://github.com/advorange/ImageDL)";
		/// <inheritdoc />
		public List<IImageGatherer> Gatherers { get; }
		/// <inheritdoc />
		public Dictionary<Type, ApiKey> ApiKeys { get; }
		/// <inheritdoc />
		public CookieContainer Cookies { get; }

		/// <summary>
		/// Creates an instance of <see cref="DownloaderClient"/>.
		/// </summary>
		public DownloaderClient() : this(new CookieContainer()) { }
		/// <summary>
		/// Creates an instance of <see cref="DownloaderClient"/>.
		/// </summary>
		public DownloaderClient(CookieContainer cookies) : base(GetDefaultClientHandler(cookies))
		{
			Gatherers = typeof(IImageGatherer).Assembly.DefinedTypes
				.Where(x => !x.IsAbstract && x.IsValueType && x.ImplementedInterfaces.Contains(typeof(IImageGatherer)))
				.Select(x => (IImageGatherer)Activator.CreateInstance(x))
				.ToList();
			ApiKeys = new Dictionary<Type, ApiKey>();
			Cookies = cookies;
			Timeout = TimeSpan.FromMilliseconds(1000 * 60 * 5);

			DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			DefaultRequestHeaders.Add("Accept-Encoding", "gzip, default, br");
			DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9"); //Make sure we get English results
			DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			DefaultRequestHeaders.Add("Connection", "keep-alive");
			DefaultRequestHeaders.Add("pragma", "no-cache");
			DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			DefaultRequestHeaders.Add("User-Agent", UserAgent);
		}

		/// <summary>
		/// Removes query parameters and hash parameters from a url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static Uri RemoveQuery(Uri url)
		{
			return new Uri(url.ToString().Split('?', '#')[0]);
		}
		/// <summary>
		/// Generates the default client handler for the client.
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		private static HttpClientHandler GetDefaultClientHandler(CookieContainer cookies)
		{
			cookies.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter
			cookies.Add(new Cookie("ig_pr", "1", "/", "www.instagram.com")); //Otherwise Instagram gives 403 errors

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
		public HttpRequestMessage GenerateReq(Uri url, HttpMethod method = default)
		{
			var req = new HttpRequestMessage
			{
				RequestUri = url,
				Method = method ?? HttpMethod.Get,
			};
			req.Headers.Referrer = url; //Set self as referer since Pixiv requires a valid Pixiv url as its referer
			return req;
		}
		/// <inheritdoc />
		public async Task<ClientResult<string>> GetTextAsync(Func<HttpRequestMessage> reqFactory, TimeSpan wait = default, int tries = 3)
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

				using (var resp = await SendAsync(reqFactory.Invoke()).CAF())
				{
					var code = (int)resp.StatusCode;
					if (code == 421 || code == 429) //Rate limit error codes
					{
						//Wait longer on each failure
						nextRetry = DateTime.UtcNow.Add(TimeSpan.FromTicks(wait.Ticks * (int)Math.Pow(2, i)));
						ConsoleUtils.WriteLine($"Rate limited; retrying next at: {nextRetry.ToLongTimeString()}");
						continue;
					}

					//Means this is using the brotli compression method, which isn't implemented into HttpClient yet
					if (resp.Content.Headers.TryGetValues("Content-Encoding", out var val) && val.Any(x => x == "br"))
					{
						using (var s = await resp.Content.ReadAsStreamAsync().CAF())
						using (var br = new BrotliStream(s, CompressionMode.Decompress))
						using (var r = new StreamReader(br))
						{
							return new ClientResult<string>(await r.ReadToEndAsync().CAF(), resp.StatusCode, resp.IsSuccessStatusCode);
						}
					}

					var bytes = await resp.Content.ReadAsByteArrayAsync().CAF();
					var text = Encoding.UTF8.GetString(bytes);
					return new ClientResult<string>(text, resp.StatusCode, resp.IsSuccessStatusCode);
				}
			}
			throw new HttpRequestException($"Unable to get the requested webpage after {tries} tries.");
		}
		/// <inheritdoc />
		public async Task<ClientResult<HtmlDocument>> GetHtmlAsync(Func<HttpRequestMessage> reqFactory, TimeSpan wait = default, int tries = 3)
		{
			var result = await GetTextAsync(reqFactory, wait, tries).CAF();
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
		public void AddGatherer<T>() where T : IImageGatherer, new()
		{
			Gatherers.Add(new T());
		}
		/// <inheritdoc />
		public void RemoveGatherer<T>() where T : IImageGatherer, new()
		{
			Gatherers.RemoveAll(x => x is T);
		}
	}
}