using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Results of searching for posts.
	/// </summary>
	public struct ArtstationPage
	{
		/// <summary>
		/// The current posts.
		/// </summary>
		[JsonProperty("data")]
		public IList<ArtstationPost> Posts { get; private set; }

		/// <summary>
		/// The total amount of posts from a user.
		/// </summary>
		[JsonProperty("total_count")]
		public int TotalCount { get; private set; }
	}
}