#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the comments of a post.
	/// </summary>
	public struct CommentInfo
	{
		[JsonProperty("count")]
		public int Count;
		[JsonProperty("page_info")]
		public PageInfo PageInfo;
		[JsonProperty("edges")]
		public List<CommentNode> Nodes;
	}

	/// <summary>
	/// Holds the comment.
	/// </summary>
	public struct CommentNode
	{
		[JsonProperty("node")]
		public readonly Comment Comment;
	}

	/// <summary>
	/// Information of a comment.
	/// </summary>
	public struct Comment
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("text")]
		public readonly string Text;
		[JsonProperty("created_at")]
		public readonly long CreatedAtTimestamp;
		[JsonProperty("owner")]
		public readonly User Owner;

		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
	}
}
