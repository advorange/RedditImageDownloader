using System;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about a page.
	/// </summary>
	public struct WeiboPageInfo
	{
		/// <summary>
		/// First description provided.
		/// </summary>
		[JsonProperty("content1")]
		public string FirstContent { get; private set; }

		/// <summary>
		/// If the post is a video, this will have a link.
		/// </summary>
		[JsonProperty("media_info")]
		public WeiboMediaInfo MediaInfo { get; private set; }

		/// <summary>
		/// The unique id of the page.
		/// </summary>
		[JsonProperty("object_id")]
		public string ObjectId { get; private set; }

		/// <summary>
		/// The url to the page's image.
		/// </summary>
		[JsonProperty("page_pic")]
		[JsonConverter(typeof(WeiboPagePictureConverter))]
		public Uri PagePictureUrl { get; private set; }

		/// <summary>
		/// The title of the page.
		/// </summary>
		[JsonProperty("page_title")]
		public string PageTitle { get; private set; }

		/// <summary>
		/// The url to the page.
		/// </summary>
		[JsonProperty("page_url")]
		public Uri PageUrl { get; private set; }

		/// <summary>
		/// Second description provided.
		/// </summary>
		[JsonProperty("content2")]
		public string SecondContent { get; private set; }

		/// <summary>
		/// The type of page, e.g. video, topic, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// Returns the page url.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => PageUrl.ToString();
	}
}