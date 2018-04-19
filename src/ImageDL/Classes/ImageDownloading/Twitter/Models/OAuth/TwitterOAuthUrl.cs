using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Links in the post.
	/// </summary>
	public struct TwitterOAuthUrl
	{
		/// <summary>
		/// Where the url starts and ends in the text.
		/// </summary>
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		/// <summary>
		/// Url in the text. This is a relative uri.
		/// </summary>
		[JsonProperty("display_url")]
		public string DisplayUrl { get; private set; }
		/// <summary>
		/// Includes the http/https if not already included.
		/// </summary>
		[JsonProperty("expanded_url")]
		public Uri ExpandedUrl { get; private set; }
		/// <summary>
		/// The url converted into a Twitter url.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
	}
}