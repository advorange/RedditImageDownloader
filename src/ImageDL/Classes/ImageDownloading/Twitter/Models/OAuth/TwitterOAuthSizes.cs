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
	public struct TwitterOAuthSize
	{
		/// <summary>
		/// Width of this size.
		/// </summary>
		[JsonProperty("w")]
		public int Width { get; private set; }
		/// <summary>
		/// Height of this size.
		/// </summary>
		[JsonProperty("h")]
		public int Height { get; private set; }
		/// <summary>
		/// Resizing method for this size, e.g. crop, etc.
		/// </summary>
		[JsonProperty("resize")]
		public string Resize { get; private set; }
	}
}