#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Holds information about a user. Not all of it will be populated, depending on where it was found in the Json.
	/// </summary>
	public struct User
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("profile_pic_url")]
		public readonly string ProfilePicUrl;
		[JsonProperty("username")]
		public readonly string Username;
		[JsonProperty("blocked_by_viewer")]
		public readonly bool BlockedByViewer;
		[JsonProperty("followed_by_viewer")]
		public readonly bool FollowedByViewer;
		[JsonProperty("full_name")]
		public readonly string FullName;
		[JsonProperty("has_blocked_viewer")]
		public readonly bool HasBlockedViewer;
		[JsonProperty("is_private")]
		public readonly bool IsPrivate;
		[JsonProperty("is_unpublished")]
		public readonly bool IsUnpublished;
		[JsonProperty("is_verified")]
		public readonly bool IsVerified;
		[JsonProperty("requested_by_viewer")]
		public readonly bool RequestedByViewer;
	}
}
