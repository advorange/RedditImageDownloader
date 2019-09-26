using System;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.OAuth
{
	/// <summary>
	/// Holds information about the image.
	/// </summary>
	public struct DeviantArtOAuthImage : ISize
	{
		/// <summary>
		/// The size of the thumbnail in bytes.
		/// </summary>
		[JsonProperty("filesize")]
		public long FileSize { get; private set; }

		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// Whether or not the thumbnail is transparent.
		/// </summary>
		[JsonProperty("transparency")]
		public bool IsTransparent { get; private set; }

		/// <summary>
		/// The direct link to the thumbnail.
		/// </summary>
		[JsonProperty("src")]
		public Uri Source { get; private set; }

		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }

		/// <summary>
		/// Returns with width and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Width}x{Height}";
	}
}