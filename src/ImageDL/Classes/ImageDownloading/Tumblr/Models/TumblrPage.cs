using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for the results of a Tumblr page.
	/// </summary>
	public class TumblrPage
	{
		/// <summary>
		/// The owner of the page.
		/// </summary>
		[JsonProperty("tumblelog")]
		public readonly TumblrPageOwner Owner;
		/// <summary>
		/// The start of current page.
		/// </summary>
		[JsonProperty("posts-start")]
		public readonly int PostsStart;
		/// <summary>
		/// The total amount of posts this user has made.
		/// </summary>
		[JsonProperty("posts-total")]
		public readonly int PostsTotal;
		/// <summary>
		/// The type of posts to search for.
		/// </summary>
		[JsonProperty("posts-type")]
		public readonly string PostsType;
		/// <summary>
		/// The posts that were found.
		/// </summary>
		[JsonProperty("posts")]
		public readonly List<TumblrPost> Posts;
	}
}