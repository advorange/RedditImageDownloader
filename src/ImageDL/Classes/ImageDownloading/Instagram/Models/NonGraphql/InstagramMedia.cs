#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds the node leading to the information of a post.
	/// </summary>
	public sealed class InstagramMedia
	{
		[JsonProperty("node")]
		public readonly InstagramMediaNode Node;

		/// <inheritdoc />
		public override string ToString()
		{
			return Node.ToString();
		}
	}
}
