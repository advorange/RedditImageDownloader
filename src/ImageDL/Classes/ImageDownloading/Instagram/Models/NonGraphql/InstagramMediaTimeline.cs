#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information about the media gotten from the query.
	/// </summary>
	public sealed class InstagramMediaTimeline
	{
		[JsonProperty("count")]
		public readonly int Count;
		[JsonProperty("page_info")]
		public readonly InstagramPageInfo PageInfo;
		[JsonProperty("edges")]
		public readonly List<InstagramMedia> Posts;
	}
}
