using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	public class TumblrPage
	{
		[JsonProperty("tumblelog")]
		public readonly TumblrPageOwner Owner;
		[JsonProperty("posts-start")]
		public readonly int PostsStart;
		[JsonProperty("posts-total")]
		public readonly int PostsTotal;
		[JsonProperty("posts-type")]
		public readonly string PostsType;
		[JsonProperty("posts")]
		public readonly List<TumblrPost> Posts;
	}
}
