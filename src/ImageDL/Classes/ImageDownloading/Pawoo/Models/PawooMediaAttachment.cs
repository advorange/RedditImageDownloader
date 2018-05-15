using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Information about an image or video.
	/// </summary>
	public struct PawooMediaAttachment
	{
		/// <summary>
		/// The id of the image.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		/// The type of media, e.g. image, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The direct url to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The url to the thumbnail.
		/// </summary>
		[JsonProperty("preview_url")]
		public Uri PreviewUrl { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("remote_url")]
		public Uri RemoteUrl { get; private set; }
		/// <summary>
		/// Not sure either, redirects to image.
		/// </summary>
		[JsonProperty("text_url")]
		public Uri TextUrl { get; private set; }
		/// <summary>
		/// Meta information, such as size.
		/// </summary>
		[JsonProperty("meta")]
		public PawooMeta Meta { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("description")]
		public object Description { get; private set; }
	}
}