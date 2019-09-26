using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// The user who posted the Pinterest post.
	/// </summary>
	public struct PinterestUser
	{
		/// <summary>
		/// The user's first name.
		/// </summary>
		[JsonProperty("first_name")]
		public string FirstName { get; private set; }

		/// <summary>
		/// The user's full name.
		/// </summary>
		[JsonProperty("full_name")]
		public string FullName { get; private set; }

		/// <summary>
		/// The user's gender.
		/// </summary>
		[JsonProperty("gender")]
		public string Gender { get; private set; }

		/// <summary>
		/// The id of the user.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// The user's profile picture, large sized.
		/// </summary>
		[JsonProperty("image_large_url")]
		public string ImageLargeUrl { get; private set; }

		/// <summary>
		/// The user's profile picture, medium sized.
		/// </summary>
		[JsonProperty("image_medium_url")]
		public string ImageMediumUrl { get; private set; }

		/// <summary>
		/// The user's profile picture, small sized.
		/// </summary>
		[JsonProperty("image_small_url")]
		public string ImageSmallUrl { get; private set; }

		/// <summary>
		/// The user's profile picture, largest sized.
		/// </summary>
		[JsonProperty("image_xlarge_url")]
		public string ImageXlargeUrl { get; private set; }

		/// <summary>
		/// The user's last name.
		/// </summary>
		[JsonProperty("last_name")]
		public string LastName { get; private set; }

		/// <summary>
		/// The type of object, e.g. user.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// The user's display name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// Returns the username, and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Username} ({Id})";
	}
}