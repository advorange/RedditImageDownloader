using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	/// <summary>
	/// Json model for the owner of a Tumblr post.
	/// </summary>
	public class TumblrPostOwner
	{
		/// <summary>
		/// The title of the user's page.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The name of the user.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// Whether the user has a custom domain.
		/// </summary>
		[JsonProperty("cname")]
		public bool Cname { get; private set; }
		/// <summary>
		/// A link to the user's page.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The timezone the user is in.
		/// </summary>
		[JsonProperty("timezone")]
		public string Timezone { get; private set; }
		/// <summary>
		/// A link to the user's profile picture.
		/// </summary>
		[JsonProperty("avatar_url_512")]
		public Uri AvatarUrl { get; private set; }

		/// <summary>
		/// Returns the name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}
}