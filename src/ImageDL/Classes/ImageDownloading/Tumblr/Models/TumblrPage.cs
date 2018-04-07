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
		public TumblrPageOwner Owner { get; private set; }
		/// <summary>
		/// The start of current page.
		/// </summary>
		[JsonProperty("posts-start")]
		public int PostsStart { get; private set; }
		/// <summary>
		/// The total amount of posts this user has made.
		/// </summary>
		[JsonProperty("posts-total")]
		public int PostsTotal { get; private set; }
		/// <summary>
		/// The type of posts to search for.
		/// </summary>
		[JsonProperty("posts-type")]
		public string PostsType { get; private set; }
		/// <summary>
		/// The posts that were found.
		/// </summary>
		[JsonProperty("posts")]
		public List<TumblrPost> Posts { get; private set; }
	}
}