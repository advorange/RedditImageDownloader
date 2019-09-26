using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Metadata for a post from Pinterest.
	/// </summary>
	public sealed class PinterestRichMetadata
	{
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("amp_url")]
		public string AmpUrl { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("amp_valid")]
		public bool AmpValid { get; private set; }

		/// <summary>
		/// Urls to the icon for the source.
		/// </summary>
		[JsonProperty("apple_touch_icon_images")]
		public IDictionary<string, Uri> AppleTouchIconImages { get; private set; }

		/// <summary>
		/// Url to the icon for the source.
		/// </summary>
		[JsonProperty("apple_touch_icon_link")]
		public Uri AppleTouchIconLink { get; private set; }

		/// <summary>
		/// Indicates the source has article information. See https://developers.pinterest.com/docs/rich-pins/articles/ for more details.
		/// </summary>
		[JsonProperty("article")]
		public PinterestArticle Article { get; private set; }

		/// <summary>
		/// Description of the source.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }

		/// <summary>
		/// Urls to the icon for the source.
		/// </summary>
		[JsonProperty("favicon_images")]
		public IDictionary<string, Uri> FaviconImages { get; private set; }

		/// <summary>
		/// Url to the icon for the source.
		/// </summary>
		[JsonProperty("favicon_link")]
		public string FaviconLink { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("has_price_drop")]
		public bool HasPriceDrop { get; private set; }

		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("link_status")]
		public int LinkStatus { get; private set; }

		/// <summary>
		/// The language to use.
		/// </summary>
		[JsonProperty("locale")]
		public string Locale { get; private set; }

		/// <summary>
		/// The host of the source of the post.
		/// </summary>
		[JsonProperty("site_name")]
		public string SiteName { get; private set; }

		/// <summary>
		/// The host url title.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("tracker")]
		public object Tracker { get; private set; }

		/// <summary>
		/// The type of object this is, e.g. richpindataview.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// The url to the source.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }

		/// <summary>
		/// Returns the site name, and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{SiteName} ({Id})";
	}
}