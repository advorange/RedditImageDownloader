#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the tagged users of a post.
	/// </summary>
	public struct TaggedUserInfo
	{
		[JsonProperty("edges")]
		public readonly List<TaggedUserNode> Nodes;
	}

	/// <summary>
	/// Holds the tagged user.
	/// </summary>
	public struct TaggedUserNode
	{
		[JsonProperty("node")]
		public readonly TaggedUser TaggedUser;
	}

	/// <summary>
	/// Information of a tagged user.
	/// </summary>
	public struct TaggedUser
	{
		[JsonProperty("user")]
		public readonly User User;
		[JsonProperty("x")]
		public readonly double X;
		[JsonProperty("y")]
		public readonly double Y;
	}
}
