using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// A page in a Pixiv post.
	/// </summary>
	public sealed class PixivPostPage
	{
		/// <summary>
		/// The urls of the page.
		/// </summary>
		[JsonProperty("image_urls")]
		public IDictionary<string, Uri> ImageUrls { get; private set; }
	}
}