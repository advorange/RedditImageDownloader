#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Current results and the status of an Instagram query.
	/// </summary>
	public sealed class InstagramResult
	{
		[JsonProperty("data")]
		public readonly Data Data;
		[JsonProperty("status")]
		public readonly string Status;
	}

	/// <summary>
	/// Holds information.
	/// </summary>
	public sealed class Data
	{
		[JsonProperty("user")]
		public readonly UserInfo User;
	}

	/// <summary>
	/// Holds information relating to the user.
	/// </summary>
	public sealed class UserInfo
	{
		[JsonProperty("edge_owner_to_timeline_media")]
		public readonly MediaTimeline Content;
	}

	/// <summary>
	/// Holds information about the media gotten from the query.
	/// </summary>
	public sealed class MediaTimeline
	{
		[JsonProperty("count")]
		public readonly int Count;
		[JsonProperty("page_info")]
		public readonly PageInfo PageInfo;
		[JsonProperty("edges")]
		public readonly List<Media> Posts;
	}

	/// <summary>
	/// Holds the node leading to the information of a post.
	/// </summary>
	public sealed class Media
	{
		[JsonProperty("node")]
		public readonly MediaNode Node;

		/// <inheritdoc />
		public override string ToString()
		{
			return Node.ToString();
		}
	}
}