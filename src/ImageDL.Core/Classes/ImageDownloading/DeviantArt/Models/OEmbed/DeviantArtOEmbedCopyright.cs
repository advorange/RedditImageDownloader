using System;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OEmbed
{
	/// <summary>
	/// Who owns this image.
	/// </summary>
	public struct DeviantArtOEmbedCopyright
	{
		/// <summary>
		/// Information about the copyright.
		/// </summary>
		[JsonProperty("_attributes")]
		public DeviantArtOEmbedCopyrightAttributes Attributes { get; private set; }
	}

	/// <summary>
	/// Information about the copyright.
	/// </summary>
	public struct DeviantArtOEmbedCopyrightAttributes
	{
		/// <summary>
		/// The author's name.
		/// </summary>
		[JsonProperty("entity")]
		public string Entity { get; private set; }

		/// <summary>
		/// The author's url.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }

		/// <summary>
		/// The year it was posted.
		/// </summary>
		[JsonProperty("year")]
		public string Year { get; private set; }
	}
}