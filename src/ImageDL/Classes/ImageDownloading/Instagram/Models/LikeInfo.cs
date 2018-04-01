#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the likes of a post.
	/// </summary>
	public struct LikeInfo
	{
		[JsonProperty("count")]
		public readonly int Count;
		[JsonProperty("edges")]
		public readonly List<LikeNode> Nodes;
	}

	/// <summary>
	/// Holds the like.
	/// </summary>
	public struct LikeNode
	{
		[JsonProperty("node")]
		public readonly Like Like;
	}

	/// <summary>
	/// Information of a like.
	/// </summary>
	public struct Like
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("profile_pic_url")]
		public readonly string ProfilePicUrl;
		[JsonProperty("username")]
		public readonly string Username;
	}
}
