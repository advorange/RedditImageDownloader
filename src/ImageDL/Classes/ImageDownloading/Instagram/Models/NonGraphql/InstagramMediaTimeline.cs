using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information about the media gotten from the query.
	/// </summary>
	public sealed class InstagramMediaTimeline
	{
		/// <summary>
		/// How many posts the users has made.
		/// </summary>
		[JsonProperty("count")]
		public readonly int Count;
		/// <summary>
		/// For paginating through the posts.
		/// </summary>
		[JsonProperty("page_info")]
		public readonly InstagramPageInfo PageInfo;
		/// <summary>
		/// The posts a user has made.
		/// </summary>
		[JsonProperty("edges")]
		public readonly List<InstagramMedia> Posts;
	}
}