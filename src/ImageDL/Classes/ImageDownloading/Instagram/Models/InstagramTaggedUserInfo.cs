#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the tagged users of a post.
	/// </summary>
	public struct InstagramTaggedUserInfo
	{
		[JsonProperty("edges")]
		public readonly List<InstagramTaggedUserNode> Nodes;
	}

	/// <summary>
	/// Holds the tagged user.
	/// </summary>
	public struct InstagramTaggedUserNode
	{
		[JsonProperty("node")]
		public readonly InstagramTaggedUser TaggedUser;

		/// <inheritdoc />
		public override string ToString()
		{
			return TaggedUser.ToString();
		}
	}

	/// <summary>
	/// Information of a tagged user.
	/// </summary>
	public struct InstagramTaggedUser
	{
		[JsonProperty("user")]
		public readonly InstagramUser User;
		[JsonProperty("x")]
		public readonly double X;
		[JsonProperty("y")]
		public readonly double Y;

		/// <inheritdoc />
		public override string ToString()
		{
			return User.ToString();
		}
	}
}
