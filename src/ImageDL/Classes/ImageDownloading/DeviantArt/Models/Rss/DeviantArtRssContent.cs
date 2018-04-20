using System;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Holds information about the image of a post.
	/// </summary>
	public struct DeviantArtRssContent : ISize
	{
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// A link to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The type of content, e.g. image.
		/// </summary>
		[JsonProperty("medium")]
		public string Medium { get; private set; }
	}
}