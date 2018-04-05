#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the children of a post.
	/// </summary>
	public struct InstagramChildrenInfo
	{
		[JsonProperty("edges")]
		public readonly List<InstagramChildNode> Nodes;
	}

	/// <summary>
	/// Holds the child.
	/// </summary>
	public struct InstagramChildNode
	{
		[JsonProperty("node")]
		public readonly InstagramMediaNode Child;

		/// <inheritdoc />
		public override string ToString()
		{
			return Child.ToString();
		}
	}
}
