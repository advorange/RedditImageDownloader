using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Json model for a user from Pawoo.
	/// </summary>
	public struct PawooUser
	{
		/// <summary>
		/// Not sure, same as <see cref="Username"/>.
		/// </summary>
		[JsonProperty("acct")]
		public string Acct { get; private set; }

		/// <summary>
		/// Link to the user's avatar image.
		/// </summary>
		[JsonProperty("avatar")]
		public Uri AvatarUrl { get; private set; }

		/// <summary>
		/// When the account was created.
		/// </summary>
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }

		/// <summary>
		/// The name that displays to everyone.
		/// </summary>
		[JsonProperty("display_name")]
		public string DisplayName { get; private set; }

		/// <summary>
		/// How many people are following them.
		/// </summary>
		[JsonProperty("followers_count")]
		public int FollowersCount { get; private set; }

		/// <summary>
		/// How many people they're following.
		/// </summary>
		[JsonProperty("following_count")]
		public int FollowingCount { get; private set; }

		/// <summary>
		/// Link to the user's header image.
		/// </summary>
		[JsonProperty("header")]
		public Uri HeaderUrl { get; private set; }

		/// <summary>
		/// The user's unique id.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Whether the profile is locked.
		/// </summary>
		[JsonProperty("locked")]
		public bool Locked { get; private set; }

		/// <summary>
		/// HTML info.
		/// </summary>
		[JsonProperty("note")]
		public string Note { get; private set; }

		/// <summary>
		/// How they were authenticated.
		/// </summary>
		[JsonProperty("oauth_authentications")]
		public IList<PawooOAuthAuthentication> OauthAuthentications { get; private set; }

		/// <summary>
		/// Link to the user's avatar image.
		/// </summary>
		[JsonProperty("avatar_static")]
		public Uri StaticAvatarUrl { get; private set; }

		/// <summary>
		/// Link to the user's header image.
		/// </summary>
		[JsonProperty("header_static")]
		public Uri StaticHeaderUrl { get; private set; }

		/// <summary>
		/// How many posts this user has made.
		/// </summary>
		[JsonProperty("statuses_count")]
		public int StatusesCount { get; private set; }

		/// <summary>
		/// Link to the user's profile.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// Returns the display name, and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{DisplayName} ({Id})";
	}
}