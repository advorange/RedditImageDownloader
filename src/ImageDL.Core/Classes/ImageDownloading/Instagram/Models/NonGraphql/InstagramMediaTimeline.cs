using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information about the media gotten from the query.
	/// </summary>
	public struct InstagramMediaTimeline
	{
		/// <summary>
		/// How many posts the users has made.
		/// </summary>
		[JsonProperty("count")]
		public int Count { get; private set; }

		/// <summary>
		/// For paginating through the posts.
		/// </summary>
		[JsonProperty("page_info")]
		public InstagramPageInfo PageInfo { get; private set; }

		/// <summary>
		/// The posts a user has made.
		/// </summary>
		[JsonProperty("edges")]
		public IList<InstagramMedia> Posts { get; private set; }
	}
}