using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Sizes for media.
	/// </summary>
	public struct TwitterOAuthSizes
	{
		/// <summary>
		/// Thumbnail size.
		/// </summary>
		[JsonProperty("thumb")]
		public TwitterOAuthSize Thumb { get; private set; }
		/// <summary>
		/// Large size.
		/// </summary>
		[JsonProperty("large")]
		public TwitterOAuthSize Large { get; private set; }
		/// <summary>
		/// Medium size.
		/// </summary>
		[JsonProperty("medium")]
		public TwitterOAuthSize Medium { get; private set; }
		/// <summary>
		/// Small size.
		/// </summary>
		[JsonProperty("small")]
		public TwitterOAuthSize Small { get; private set; }
	}

	/// <summary>
	/// Size for media.
	/// </summary>
	public struct TwitterOAuthSize : ISize
	{
		/// <inheritdoc />
		[JsonProperty("w")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("h")]
		public int Height { get; private set; }
		/// <summary>
		/// Resizing method for this size, e.g. crop, etc.
		/// </summary>
		[JsonProperty("resize")]
		public string Resize { get; private set; }
	}
}