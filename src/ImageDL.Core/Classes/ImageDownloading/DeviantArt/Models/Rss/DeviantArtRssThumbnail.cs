using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about a post's thumbnail.
	/// </summary>
	public struct DeviantArtRssThumbnail
	{
		/// <summary>
		/// The link to the thumbnail.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The thumbnail's height.
		/// </summary>
		[JsonProperty("height")]
		public string Height { get; private set; }
		/// <summary>
		/// The thumbnail's width.
		/// </summary>
		[JsonProperty("width")]
		public string Width { get; private set; }
	}
}