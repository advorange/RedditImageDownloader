using HtmlAgilityPack;
using ImageDL.Classes;
using ImageDL.Classes.ImageDownloading;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for the client used to download images.
	/// </summary>
	public interface IImageDownloaderClient
	{
		/// <summary>
		/// Regex for checking if a uri leads to animated content (video, gif, etc).
		/// </summary>
		List<Regex> AnimatedContentDomains { get; set; }
		/// <summary>
		/// Scrapers for gathering images from websites.
		/// </summary>
		List<IImageGatherer> Gatherers { get; set; }
		/// <summary>
		/// Holds api keys for specific websites.
		/// </summary>
		Dictionary<Type, ApiKey> ApiKeys { get; set; }
		/// <summary>
		/// Holds the cookies for the client.
		/// </summary>
		CookieContainer Cookies { get; }

		/// <summary>
		/// Sends a request to the uri with the specified method and the referer as itself.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		Task<HttpResponseMessage> SendWithRefererAsync(Uri uri, HttpMethod method);
		/// <summary>
		/// Sends a GET request to get the main text of the link. Waits for the passed in wait time multiplied by 2 for each failure.
		/// Will throw if tries are used up/all errors other than 421 and 429.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="wait">The amount of time to wait between retries. This will be doubled each retry.</param>
		/// <param name="tries"></param>
		/// <returns></returns>
		/// <exception cref="HttpRequestException">If unable to get the request after all retries have been used up.</exception>
		Task<ClientResult<string>> GetText(Uri uri, TimeSpan wait = default, int tries = 3);
		/// <summary>
		/// Gets the Html of a webpage.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="wait">The amount of time to wait between retries. This will be doubled each retry.</param>
		/// <param name="tries"></param>
		/// /// <returns></returns>
		/// <exception cref="HttpRequestException">If unable to get the request after all retries have been used up.</exception>
		Task<ClientResult<HtmlDocument>> GetHtml(Uri uri, TimeSpan wait = default, int tries = 3);
		/// <summary>
		/// Attempts to get images from the uri. Will only work for the specified sites.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		Task<GatheredImagesResponse> GetImagesAsync(IPost post);
	}
}
