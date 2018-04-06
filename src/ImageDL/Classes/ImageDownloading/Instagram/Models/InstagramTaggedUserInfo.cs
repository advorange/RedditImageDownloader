using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the tagged users of a post.
	/// </summary>
	public struct InstagramTaggedUserInfo
	{
		/// <summary>
		/// The users who were tagged.
		/// </summary>
		[JsonProperty("edges")]
		public readonly List<InstagramTaggedUserNode> Nodes;
	}

	/// <summary>
	/// Holds the tagged user.
	/// </summary>
	public struct InstagramTaggedUserNode
	{
		/// <summary>
		/// The user who was tagged.
		/// </summary>
		[JsonProperty("node")]
		public readonly InstagramTaggedUser TaggedUser;

		/// <summary>
		/// Returns the user as a string.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("user")]
		public readonly InstagramUser User;
		/// <summary>
		/// The x-coord of where they were tagged.
		/// </summary>
		[JsonProperty("x")]
		public readonly double X;
		/// <summary>
		/// The y-coord of where they were tagged.
		/// </summary>
		[JsonProperty("y")]
		public readonly double Y;

		/// <summary>
		/// Returns the user as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return User.ToString();
		}
	}
}