using System;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// An image on a post.
	/// </summary>
	public struct ArtstationAsset : ISize
	{
		/// <summary>
		/// The type of content this is, image, video, etc.
		/// </summary>
		[JsonProperty("asset_type")]
		public string AssetType { get; private set; }

		/// <summary>
		/// Whether the asset is a video.
		/// </summary>
		[JsonProperty("has_embedded_player")]
		public bool HasEmbeddedPlayer { get; private set; }

		/// <summary>
		/// Whether the asset is a video or image or something that shows an image.
		/// </summary>
		[JsonProperty("has_image")]
		public bool HasImage { get; private set; }

		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// The id of the image.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonProperty("image_url")]
		public Uri ImageUrl { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("oembed")]
		public object Oembed { get; private set; }

		/// <summary>
		/// OEmbed for this asset. This is in HTML.
		/// </summary>
		[JsonProperty("player_embedded")]
		public string PlayerEmbedded { get; private set; }

		/// <summary>
		/// The position the image comes in the list. This is 0 based.
		/// </summary>
		[JsonProperty("position")]
		public int Position { get; private set; }

		/// <summary>
		/// The title of the image. This is in HTML.
		/// </summary>
		[JsonProperty("title_formatted")]
		public string TitleFormatted { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("viewport_constraint_type")]
		public string ViewportConstraintType { get; private set; }

		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }

		/// <summary>
		/// Returns the url, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{ImageUrl} ({Width}x{Height})";
	}
}