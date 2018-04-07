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
		public List<InstagramCaptionNode> Nodes { get; private set; }
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
		public InstagramCaption Caption { get; private set; }

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
		public string Text { get; private set; }

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