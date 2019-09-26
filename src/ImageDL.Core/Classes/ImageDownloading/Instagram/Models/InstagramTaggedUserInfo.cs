using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information of a tagged user.
	/// </summary>
	public struct InstagramTaggedUser
	{
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("user")]
		public InstagramUser User { get; private set; }

		/// <summary>
		/// The x-coord of where they were tagged.
		/// </summary>
		[JsonProperty("x")]
		public double X { get; private set; }

		/// <summary>
		/// The y-coord of where they were tagged.
		/// </summary>
		[JsonProperty("y")]
		public double Y { get; private set; }

		/// <summary>
		/// Returns the user as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => User.ToString();
	}

	/// <summary>
	/// Information about the tagged users of a post.
	/// </summary>
	public struct InstagramTaggedUserInfo
	{
		/// <summary>
		/// The users who were tagged.
		/// </summary>
		[JsonProperty("edges")]
		public IList<InstagramTaggedUserNode> Nodes { get; private set; }
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
		public InstagramTaggedUser TaggedUser { get; private set; }

		/// <summary>
		/// Returns the user as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => TaggedUser.ToString();
	}
}