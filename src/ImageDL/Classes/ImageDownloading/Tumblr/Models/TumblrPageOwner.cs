using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for the owner of a Tumblr page.
	/// </summary>
	public class TumblrPageOwner
	{
		/// <summary>
		/// The title of the user's page.
		/// </summary>
		[JsonProperty("title")]
		public readonly string Title;
		/// <summary>
		/// The description of the user's page.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("name")]
		public readonly string Name;
		/// <summary>
		/// The timezone the user is in.
		/// </summary>
		[JsonProperty("timezone")]
		public readonly string Timezone;
		/// <summary>
		/// Whether the user has a custom domain.
		/// </summary>
		[JsonProperty("cname")]
		public readonly bool Cname;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("feeds")]
		public readonly List<object> Feeds;
	}
}