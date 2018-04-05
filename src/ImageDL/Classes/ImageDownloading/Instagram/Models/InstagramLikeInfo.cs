#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the likes of a post.
	/// </summary>
	public struct InstagramLikeInfo
	{
		[JsonProperty("count")]
		public readonly int Count;
		[JsonProperty("edges")]
		public readonly List<InstagramLikeNode> Nodes;
	}

	/// <summary>
	/// Holds the like.
	/// </summary>
	public struct InstagramLikeNode
	{
		[JsonProperty("node")]
		public readonly InstagramLike Like;

		/// <inheritdoc />
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
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("profile_pic_url")]
		public readonly string ProfilePicUrl;
		[JsonProperty("username")]
		public readonly string Username;

		/// <inheritdoc />
		public override string ToString()
		{
			return Username;
		}
	}
}
