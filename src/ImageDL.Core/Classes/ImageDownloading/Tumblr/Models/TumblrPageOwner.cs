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
		public string Title { get; private set; }
		/// <summary>
		/// The description of the user's page.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The timezone the user is in.
		/// </summary>
		[JsonProperty("timezone")]
		public string Timezone { get; private set; }
		/// <summary>
		/// Whether the user has a custom domain.
		/// </summary>
		[JsonProperty("cname")]
		public bool Cname { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("feeds")]
		public IList<object> Feeds { get; private set; }

		/// <summary>
		/// Returns the name.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Name;
	}
}