#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the captions of a post.
	/// </summary>
	public struct CaptionInfo
	{
		[JsonProperty("edges")]
		public readonly List<CaptionNode> Nodes;
	}

	/// <summary>
	/// Holds the caption.
	/// </summary>
	public struct CaptionNode
	{
		[JsonProperty("node")]
		public readonly Caption Caption;

		/// <inheritdoc />
		public override string ToString()
		{
			return Caption.ToString();
		}
	}

	/// <summary>
	/// Information of a caption.
	/// </summary>
	public struct Caption
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
