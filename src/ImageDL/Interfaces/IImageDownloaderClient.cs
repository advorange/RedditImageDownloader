using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ImageDL.Classes;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for the client used to download images.
	/// </summary>
	public interface IImageDownloaderClient
	{
		/// <summary>
		/// Scrapers for gathering images from websites.
		/// </summary>
		List<IImageGatherer> Gatherers { get; }
		/// <summary>
		/// Holds api keys for specific websites.
		/// </summary>
		Dictionary<Type, ApiKey> ApiKeys { get; }
		/// <summary>
		/// Holds the cookies for the client.
		/// </summary>
		CookieContainer Cookies { get; }

		/// <summary>
		/// Sends a request to the uri with the specified method and the referer as itself.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		HttpRequestMessage GetReq(Uri url, HttpMethod method = default);
		/// <summary>
		/// Sends a GET request to get the main text of the link. Waits for the passed in wait time multiplied by 2 for each failure.
		/// Will throw if tries are used up/all errors other than 421 and 429.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="wait">The amount of time to wait between retries. This will be doubled each retry.</param>
		/// <param name="tries"></param>
		/// <returns></returns>
		/// <exception cref="HttpRequestException">If unable to get the request after all retries have been used up.</exception>
		Task<ClientResult<string>> GetText(HttpRequestMessage req, TimeSpan wait = default, int tries = 3);
		/// <summary>
		/// Gets the Html of a webpage.
		/// </summary>
		/// <param name="req"></param>
		/// <param name="wait">The amount of time to wait between retries. This will be doubled each retry.</param>
		/// <param name="tries"></param>
		/// /// <returns></returns>
		/// <exception cref="HttpRequestException">If unable to get the request after all retries have been used up.</exception>
		Task<ClientResult<HtmlDocument>> GetHtml(HttpRequestMessage req, TimeSpan wait = default, int tries = 3);
		/// <summary>
		/// Sends a request to the supplied uri.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage req);
		/// <summary>
		/// Adds a gatherer of the specified type to the client.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		void AddGatherer<T>() where T : IImageGatherer, new();
		/// <summary>
		/// Removes a gatherer of the specified type from the client.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		void RemoveGatherer<T>() where T : IImageGatherer, new();
	}
}