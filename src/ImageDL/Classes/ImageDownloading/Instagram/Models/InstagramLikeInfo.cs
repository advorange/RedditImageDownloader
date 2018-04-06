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
		public readonly int Count;
		/// <summary>
		/// Who has liked the post.
		/// </summary>
		[JsonProperty("edges")]
		public readonly List<InstagramLikeNode> Nodes;
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
		public readonly InstagramLike Like;

		/// <summary>
		/// Returns the like as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Like.ToString();
		}
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
		public readonly string Id;
		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_pic_url")]
		public readonly string ProfilePicUrl;
		/// <summary>
		/// The name of the user.
		/// </summary>
		[JsonProperty("username")]
		public readonly string Username;

		/// <summary>
		/// Returns the username and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Username} ({Id})";
		}
	}
}