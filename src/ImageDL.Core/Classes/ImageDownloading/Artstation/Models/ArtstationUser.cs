using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information about a user on Artstation.
	/// </summary>
	public struct ArtstationUser
	{
		/// <summary>
		/// If you are followed by them. This will always be false because we're browsing anonymously.
		/// </summary>
		[JsonProperty("followed")]
		public bool Followed { get; private set; }
		/// <summary>
		/// If you are blocked by them. This will always be false because we're browsing anonymously.
		/// </summary>
		[JsonProperty("blocked")]
		public bool Blocked { get; private set; }
		/// <summary>
		/// If the user is a staff member.
		/// </summary>
		[JsonProperty("is_staff")]
		public bool IsStaff { get; private set; }
		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }
		/// <summary>
		/// A short description about the user.
		/// </summary>
		[JsonProperty("headline")]
		public string Headline { get; private set; }
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("full_name")]
		public string FullName { get; private set; }
		/// <summary>
		/// A link to a medium sized profile picture.
		/// </summary>
		[JsonProperty("medium_avatar_url")]
		public Uri MediumAvatarUrl { get; private set; }
		/// <summary>
		/// A link to a large sized profile picture.
		/// </summary>
		[JsonProperty("large_avatar_url")]
		public Uri LargeAvatarUrl { get; private set; }
		/// <summary>
		/// A link to the user's profile.
		/// </summary>
		[JsonProperty("permalink")]
		public Uri Permalink { get; private set; }
		/// <summary>
		/// Whether the user is a pro member.
		/// </summary>
		[JsonProperty("pro_member")]
		public bool ProMember { get; private set; }

		/// <summary>
		/// Returns the username, and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Username} ({Id})";
		}
	}
}
