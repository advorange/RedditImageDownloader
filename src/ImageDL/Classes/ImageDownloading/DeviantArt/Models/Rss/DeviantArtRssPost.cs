using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via an rss feed.
	/// </summary>
	public sealed class DeviantArtRssPost
	{
		[JsonProperty("title")]
		public string Title { get; private set; }
		[JsonProperty("link")]
		public Uri Link { get; private set; }
		[JsonProperty("guid")]
		public DeviantArtRssGuid Guid { get; private set; }
		[JsonProperty("pubDate")]
		public string PubDate { get; private set; }
		[JsonProperty("media:title")]
		public DeviantArtRssTitle MediaTitle { get; private set; }
		[JsonProperty("media:keywords")]
		public string MediaKeywords { get; private set; }
		[JsonProperty("media:rating")]
		public string MediaRating { get; private set; }
		[JsonProperty("media:category")]
		public DeviantArtRssCategory MediaCategory { get; private set; }
		[JsonProperty("media:credit")]
		public IList<DeviantArtRssCredit> MediaCredit { get; private set; }
		[JsonProperty("media:copyright")]
		public DeviantArtRssCopyright MediaCopyright { get; private set; }
		[JsonProperty("media:description")]
		public DeviantArtRssDescription MediaDescription { get; private set; }
		[JsonProperty("media:thumbnail")]
		public IList<DeviantArtRssThumbnail> MediaThumbnail { get; private set; }
		[JsonProperty("media:content")]
		public DeviantArtRssContent MediaContent { get; private set; }
		[JsonProperty("description")]
		public string Description { get; private set; }
	}
}