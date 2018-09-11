using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds the node leading to the information of a post.
	/// </summary>
	public sealed class InstagramMedia
	{
		/// <summary>
		/// The media.
		/// </summary>
		[JsonProperty("node")]
		public InstagramMediaNode Node { get; private set; }

		/// <summary>
		/// Returns the media as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Node.ToString();
	}
}