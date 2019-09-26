using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.AnimePictures.Models
{
	/// <summary>
	/// Results from search for images.
	/// </summary>
	public sealed class AnimePicturesPage
	{
		/// <summary>
		/// The amount of pages for the posts that fit this query.
		/// </summary>
		[JsonProperty("max_pages")]
		public int MaxPages { get; private set; }

		/// <summary>
		/// The current page.
		/// </summary>
		[JsonProperty("page_number")]
		public int PageNumber { get; private set; }

		/// <summary>
		/// The gathered posts.
		/// </summary>
		[JsonProperty("posts")]
		public IList<AnimePicturesPost> Posts { get; private set; }

		/// <summary>
		/// The amount of posts that fit this query.
		/// </summary>
		[JsonProperty("posts_count")]
		public int PostsCount { get; private set; }

		/// <summary>
		/// The amount of posts per page.
		/// </summary>
		[JsonProperty("posts_per_page")]
		public int PostsPerPage { get; private set; }

		/// <summary>
		/// The amount of posts gotten this result.
		/// </summary>
		[JsonProperty("response_posts_count")]
		public int ResponsePostsCount { get; private set; }
	}
}