using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a user. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public struct InstagramUser
	{
		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("id")]
		public readonly string Id;
		/// <summary>
		/// The link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_pic_url")]
		public readonly string ProfilePicUrl;
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public readonly string Username;
		/// <summary>
		/// If you have blocked the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("blocked_by_viewer")]
		public readonly bool BlockedByViewer;
		/// <summary>
		/// If you have followed the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("followed_by_viewer")]
		public readonly bool FollowedByViewer;
		/// <summary>
		/// The user's full name.
		/// </summary>
		[JsonProperty("full_name")]
		public readonly string FullName;
		/// <summary>
		/// If you have been blocked by the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("has_blocked_viewer")]
		public readonly bool HasBlockedViewer;
		/// <summary>
		/// Whether the user is private meaning you have to be following them to see posts.
		/// </summary>
		[JsonProperty("is_private")]
		public readonly bool IsPrivate;
		/// <summary>
		/// Whether the user deleted their account.
		/// </summary>
		[JsonProperty("is_unpublished")]
		public readonly bool IsUnpublished;
		/// <summary>
		/// Whether the user is verified to be who they claim to be.
		/// </summary>
		[JsonProperty("is_verified")]
		public readonly bool IsVerified;
		/// <summary>
		/// If you have requested to follow the user. This will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("requested_by_viewer")]
		public readonly bool RequestedByViewer;

		/// <summary>
		/// Returns the username and user id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Username} ({Id})";
		}
	}
}