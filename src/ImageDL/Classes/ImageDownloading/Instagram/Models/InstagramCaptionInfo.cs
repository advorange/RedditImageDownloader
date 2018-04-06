using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about the captions of a post.
	/// </summary>
	public struct InstagramCaptionInfo
	{
		/// <summary>
		/// The captions of a post.
		/// </summary>
		[JsonProperty("edges")]
		public readonly List<InstagramCaptionNode> Nodes;
	}

	/// <summary>
	/// Holds the caption.
	/// </summary>
	public struct InstagramCaptionNode
	{
		/// <summary>
		/// The caption.
		/// </summary>
		[JsonProperty("node")]
		public readonly InstagramCaption Caption;

		/// <summary>
		/// Returns the caption as a string.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// The text of the caption.
		/// </summary>
		[JsonProperty("text")]
		public readonly string Text;

		/// <summary>
		/// Returns the text.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Text;
		}
	}
}