#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the comments of a post.
	/// </summary>
	public struct InstagramCommentInfo
	{
		[JsonProperty("count")]
		public int Count;
		[JsonProperty("page_info")]
		public InstagramPageInfo PageInfo;
		[JsonProperty("edges")]
		public List<InstagramCommentNode> Nodes;
	}

	/// <summary>
	/// Holds the comment.
	/// </summary>
	public struct InstagramCommentNode
	{
		[JsonProperty("node")]
		public readonly InstagramComment Comment;

		/// <inheritdoc />
		public override string ToString()
		{
			return Comment.ToString();
		}
	}

	/// <summary>
	/// Information of a comment.
	/// </summary>
	public struct InstagramComment
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("text")]
		public readonly string Text;
		[JsonProperty("created_at")]
		public readonly long CreatedAtTimestamp;
		[JsonProperty("owner")]
		public readonly InstagramUser Owner;

		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Owner} ({CreatedAt})";
		}
	}
}
