using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageScraping;
using System;
using System.Threading.Tasks;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can scrape a website.
	/// </summary>
	public interface IWebsiteScraper
	{
		/// <summary>
		/// Removes query parameters from a url.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		Uri RemoveQuery(Uri uri);
		/// <summary>
		/// Attempts to get images from a uri.
		/// Will not edit the uri at all.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		Task<ScrapeResult> ScrapeAsync(ImageDownloaderClient client, Uri uri);
		/// <summary>
		/// Determines if the uri can be scraped with this scraper.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		bool IsFromWebsite(Uri uri);
		/// <summary>
		/// Edits the uri to remove unnecessary parts.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		Uri EditUri(Uri uri);
	}
}
