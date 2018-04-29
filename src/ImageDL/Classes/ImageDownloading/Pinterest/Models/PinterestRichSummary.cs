using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Summary information for a Pinterest post.
	/// </summary>
	public class PinterestRichSummary
	{
		/// <summary>
		/// The site name.
		/// </summary>
		[JsonProperty("site_name")]
		public string SiteName { get; private set; }
		/// <summary>
		/// The type of object, e.g. article.
		/// </summary>
		[JsonProperty("type_name")]
		public string TypeName { get; private set; }
		/// <summary>
		/// The author's display name.
		/// </summary>
		[JsonProperty("display_name")]
		public string DisplayName { get; private set; }
		/// <summary>
		/// The urls to the source website's icons.
		/// </summary>
		[JsonProperty("apple_touch_icon_images")]
		public IDictionary<string, Uri> AppleTouchIconImages { get; private set; }
		/// <summary>
		/// The urls to the source website's icons.
		/// </summary>
		[JsonProperty("favicon_images")]
		public IDictionary<string, Uri> FaviconImages { get; private set; }

		/// <summary>
		/// Returns the site name, and display name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{SiteName} ({DisplayName})";
		}
	}
}