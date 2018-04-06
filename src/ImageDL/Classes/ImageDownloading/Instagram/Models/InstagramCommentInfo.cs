using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the comments of a post.
	/// </summary>
	public struct InstagramCommentInfo
	{
		/// <summary>
		/// How many comments the post has.
		/// </summary>
		[JsonProperty("count")]
		public int Count;
		/// <summary>
		/// Used for paginating through the comments.
		/// </summary>
		[JsonProperty("page_info")]
		public InstagramPageInfo PageInfo;
		/// <summary>
		/// Who has commented.
		/// </summary>
		[JsonProperty("edges")]
		public List<InstagramCommentNode> Nodes;
	}

	/// <summary>
	/// Holds the comment.
	/// </summary>
	public struct InstagramCommentNode
	{
		/// <summary>
		/// The user who commented.
		/// </summary>
		[JsonProperty("node")]
		public readonly InstagramComment Comment;

		/// <summary>
		/// Returns the comment as a string.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// The id of the comment.
		/// </summary>
		[JsonProperty("id")]
		public readonly string Id;
		/// <summary>
		/// The text of the comment.
		/// </summary>
		[JsonProperty("text")]
		public readonly string Text;
		/// <summary>
		/// The unix timestamp in seconds of when the comment was made.
		/// </summary>
		[JsonProperty("created_at")]
		public readonly long CreatedAtTimestamp;
		/// <summary>
		/// Who made the comment.
		/// </summary>
		[JsonProperty("owner")]
		public readonly InstagramUser Owner;

		/// <summary>
		/// When the comment was made.
		/// </summary>
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();

		/// <summary>
		/// Returns the user as a string and the time the comment was made.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Owner} ({CreatedAt})";
		}
	}
}