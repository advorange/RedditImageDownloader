namespace ImageDL.Enums
{
	/// <summary>
	/// The method to gather posts with.
	/// </summary>
	public enum DeviantArtGatheringMethod
	{
		/// <summary>
		/// Scrape the website directly by parsing HTML.
		/// </summary>
		Scraping,
		/// <summary>
		/// Get the values from an api using either json or xml.
		/// </summary>
		Api,
		/// <summary>
		/// Get the values from an rss feed as xml.
		/// </summary>
		Rss,
	}
}
