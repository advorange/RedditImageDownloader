using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the likes of a post.
	/// </summary>
	public struct InstagramLikeInfo
	{
		/// <summary>
		/// The amount of likes a post has.
		/// </summary>
		[JsonProperty("count")]
		public int Count { get; private set; }
		/// <summary>
		/// Who has liked the post.
		/// </summary>
		[JsonProperty("edges")]
		public IList<InstagramLikeNode> Nodes { get; private set; }
	}

	/// <summary>
	/// Holds the like.
	/// </summary>
	public struct InstagramLikeNode
	{
		/// <summary>
		/// The user who liked the post.
		/// </summary>
		[JsonProperty("node")]
		public InstagramLike Like { get; private set; }

		/// <summary>
		/// Returns the like as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Like.ToString();
	}

	/// <summary>
	/// Information of a like.
	/// </summary>
	public struct InstagramLike
	{
		/// <summary>
		/// The id of the like.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_pic_url")]
		public string ProfilePicUrl { get; private set; }
		/// <summary>
		/// The name of the user.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// Returns the username and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Username} ({Id})";
	}
}