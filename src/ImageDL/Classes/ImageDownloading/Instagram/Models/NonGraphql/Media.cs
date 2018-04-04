#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds the node leading to the information of a post.
	/// </summary>
	public sealed class Media
	{
		[JsonProperty("node")]
		public readonly MediaNode Node;

		/// <inheritdoc />
		public override string ToString()
		{
			return Node.ToString();
		}
	}
}
