#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the captions of a post.
	/// </summary>
	public struct InstagramCaptionInfo
	{
		[JsonProperty("edges")]
		public readonly List<InstagramCaptionNode> Nodes;
	}

	/// <summary>
	/// Holds the caption.
	/// </summary>
	public struct InstagramCaptionNode
	{
		[JsonProperty("node")]
		public readonly InstagramCaption Caption;

		/// <inheritdoc />
		public override string ToString()
		{
			return Caption.ToString();
		}
	}

	/// <summary>
	/// Information of a caption.
	/// </summary>
	public struct InstagramCaption
	{
		[JsonProperty("text")]
		public readonly string Text;

		/// <inheritdoc />
		public override string ToString()
		{
			return Text;
		}
	}
}
