using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the children of a post.
	/// </summary>
	public struct InstagramChildrenInfo
	{
		/// <summary>
		/// The children of a post.
		/// </summary>
		[JsonProperty("edges")]
		public IList<InstagramChildNode> Nodes { get; private set; }
	}

	/// <summary>
	/// Holds the child.
	/// </summary>
	public struct InstagramChildNode
	{
		/// <summary>
		/// The child.
		/// </summary>
		[JsonProperty("node")]
		public InstagramMediaNode Child { get; private set; }

		/// <summary>
		/// Returns the image as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Child.ToString();
	}
}