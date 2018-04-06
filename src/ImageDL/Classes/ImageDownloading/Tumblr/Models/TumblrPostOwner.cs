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
		public string Title;
		/// <summary>
		/// The name of the user.
		/// </summary>
		[JsonProperty("name")]
		public string Name;
		/// <summary>
		/// Whether the user has a custom domain.
		/// </summary>
		[JsonProperty("cname")]
		public bool Cname;
		/// <summary>
		/// A link to the user's page.
		/// </summary>
		[JsonProperty("url")]
		public string Url;
		/// <summary>
		/// The timezone the user is in.
		/// </summary>
		[JsonProperty("timezone")]
		public string Timezone;
		/// <summary>
		/// A link to the user's profile picture.
		/// </summary>
		[JsonProperty("avatar_url_512")]
		public string AvatarUrl;
	}
}