using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about the image of a post.
	/// </summary>
	public struct DeviantArtRssContent
	{
		/// <summary>
		/// A link to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public string Height { get; private set; }
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("width")]
		public string Width { get; private set; }
		/// <summary>
		/// The type of content, e.g. image.
		/// </summary>
		[JsonProperty("medium")]
		public string Medium { get; private set; }
	}
}