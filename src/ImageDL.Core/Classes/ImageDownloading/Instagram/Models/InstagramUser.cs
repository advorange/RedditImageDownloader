using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a user. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public struct InstagramUser
	{
		/// <summary>
		/// If you have blocked the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("blocked_by_viewer")]
		public bool BlockedByViewer { get; private set; }

		/// <summary>
		/// If you have followed the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("followed_by_viewer")]
		public bool FollowedByViewer { get; private set; }

		/// <summary>
		/// The user's full name.
		/// </summary>
		[JsonProperty("full_name")]
		public string FullName { get; private set; }

		/// <summary>
		/// If you have been blocked by the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("has_blocked_viewer")]
		public bool HasBlockedViewer { get; private set; }

		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Whether the user is private meaning you have to be following them to see posts.
		/// </summary>
		[JsonProperty("is_private")]
		public bool IsPrivate { get; private set; }

		/// <summary>
		/// Whether the user deleted their account.
		/// </summary>
		[JsonProperty("is_unpublished")]
		public bool IsUnpublished { get; private set; }

		/// <summary>
		/// Whether the user is verified to be who they claim to be.
		/// </summary>
		[JsonProperty("is_verified")]
		public bool IsVerified { get; private set; }

		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_pic_url")]
		public string ProfilePicUrl { get; private set; }

		/// <summary>
		/// If you have requested to follow the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("requested_by_viewer")]
		public bool RequestedByViewer { get; private set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// Returns the username and user id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Username} ({Id})";
	}
}