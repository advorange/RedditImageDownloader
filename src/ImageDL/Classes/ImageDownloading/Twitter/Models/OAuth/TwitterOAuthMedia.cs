using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Media in a post.
	/// </summary>
	public struct TwitterOAuthMedia
	{
		/// <summary>
		/// Where the url starts and ends in the text.
		/// </summary>
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		/// <summary>
		/// The id of the media.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
		/// <summary>
		/// String of <see cref="Id"/>.
		/// </summary>
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		/// <summary>
		/// Url in the text. This is a relative uri.
		/// </summary>
		[JsonProperty("display_url")]
		public string DisplayUrl { get; private set; }
		/// <summary>
		/// Links to the media display.
		/// </summary>
		[JsonProperty("expanded_url")]
		public Uri ExpandedUrl { get; private set; }
		/// <summary>
		/// The url converted into a Twitter url.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// Http url leading to the media file.
		/// </summary>
		[JsonProperty("media_url")]
		public Uri MediaUrl { get; private set; }
		/// <summary>
		/// Https url leading to the media file.
		/// </summary>
		[JsonProperty("media_url_https")]
		public Uri MediaUrlHttps { get; private set; }
		/// <summary>
		/// Shows the available sizes of an image.
		/// </summary>
		[JsonProperty("sizes")]
		public TwitterOAuthSizes Sizes { get; private set; }
		/// <summary>
		/// Points to the original tweet this is from.
		/// </summary>
		[JsonProperty("source_status_id")]
		public long? SourceStatusId { get; private set; }
		/// <summary>
		/// String of <see cref="SourceStatusId"/>.
		/// </summary>
		[JsonProperty("source_status_id_str")]
		public string SourceStatusIdStr { get; private set; }
		/// <summary>
		/// The type of media, e.g. photo, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
	}
}