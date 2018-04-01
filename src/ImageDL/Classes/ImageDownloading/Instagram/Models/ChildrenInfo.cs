#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the children of a post.
	/// </summary>
	public struct ChildrenInfo
	{
		[JsonProperty("edges")]
		public readonly List<ChildNode> Nodes;
	}

	/// <summary>
	/// Holds the child.
	/// </summary>
	public struct ChildNode
	{
		[JsonProperty("node")]
		public readonly MediaNode Child;
	}
}
