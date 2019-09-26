using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Meta information about a Pawoo post.
	/// </summary>
	public struct PawooMeta
	{
		/// <summary>
		/// The focus of the post.
		/// </summary>
		[JsonProperty("focus")]
		public PawooFocus Focus { get; private set; }

		/// <summary>
		/// The size of the original post.
		/// </summary>
		[JsonProperty("original")]
		public PawooImageSize Original { get; private set; }

		/// <summary>
		/// The size of the thumbnail.
		/// </summary>
		[JsonProperty("small")]
		public PawooImageSize Small { get; private set; }

		/// <summary>
		/// Returns the image size and focus.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Original} {Focus}";
	}
}