using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Api
{
	/// <summary>
	/// Holds information about a resized version of the image.
	/// </summary>
	public struct DeviantArtApiThumbnail
	{
		/// <summary>
		/// Whether or not the thumbnail is transparent.
		/// </summary>
		[JsonProperty("transparency")]
		public bool IsTransparent { get; private set; }
		/// <summary>
		/// The thumbnail's width.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The thumbnail's height.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The size of the thumbnail in bytes.
		/// </summary>
		[JsonProperty("filesize")]
		public long FileSize { get; private set; }
		/// <summary>
		/// The direct link to the thumbnail.
		/// </summary>
		[JsonProperty("src")]
		public Uri Source { get; private set; }

		/// <summary>
		/// Returns with width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}