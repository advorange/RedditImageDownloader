using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// The pages in the post.
	/// </summary>
	public sealed class PixivPostMetadata
	{
		/// <summary>
		/// The pages in the post.
		/// </summary>
		[JsonProperty("pages")]
		public IList<PixivPostPage> Pages { get; private set; }
	}
}