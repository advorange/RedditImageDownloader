using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// The user who posted the images.
	/// </summary>
	public struct WeiboUser
	{
		/// <summary>
		/// Badges a user has.
		/// </summary>
		[JsonProperty("badge")]
		public IDictionary<string, int> Badges { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("close_blue_v")]
		public bool CloseBlueV { get; private set; }

		/// <summary>
		/// Description for the user's page.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }

		/// <summary>
		/// How many people they follow.
		/// </summary>
		[JsonProperty("follow_count")]
		public int FollowCount { get; private set; }

		/// <summary>
		/// How many followers they have.
		/// </summary>
		[JsonProperty("followers_count")]
		public int FollowersCount { get; private set; }

		/// <summary>
		/// Whether you follow them.
		/// </summary>
		[JsonProperty("following")]
		public bool Following { get; private set; }

		/// <summary>
		/// Whether they follow you.
		/// </summary>
		[JsonProperty("follow_me")]
		public bool FollowMe { get; private set; }

		/// <summary>
		/// The user's gender.
		/// </summary>
		[JsonProperty("gender")]
		public string Gender { get; private set; }

		/// <summary>
		/// Url to the full size profile picture.
		/// </summary>
		[JsonProperty("avatar_hd")]
		public Uri HighQualityProfileImageUrl { get; private set; }

		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Whether you like them.
		/// </summary>
		[JsonProperty("like")]
		public bool Like { get; private set; }

		/// <summary>
		/// Whether they like you.
		/// </summary>
		[JsonProperty("like_me")]
		public bool LikeMe { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("mbrank")]
		public int Mbrank { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("mbtype")]
		public int Mbtype { get; private set; }

		/// <summary>
		/// Url to the profile cover.
		/// </summary>
		[JsonProperty("cover_image_phone")]
		public Uri ProfileCoverUrl { get; private set; }

		/// <summary>
		/// Url to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_image_url")]
		public Uri ProfileImageUrl { get; private set; }

		/// <summary>
		/// Url to the user's profile.
		/// </summary>
		[JsonProperty("profile_url")]
		public Uri ProfileUrl { get; private set; }

		/// <summary>
		/// The user's display name.
		/// </summary>
		[JsonProperty("screen_name")]
		public string ScreenName { get; private set; }

		/// <summary>
		/// How many posts the user has made.
		/// </summary>
		[JsonProperty("statuses_count")]
		public int StatusesCount { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("urank")]
		public int Urank { get; private set; }

		/// <summary>
		/// Whether they are verified.
		/// </summary>
		[JsonProperty("verified")]
		public bool Verified { get; private set; }

		/// <summary>
		/// The reason for verification.
		/// </summary>
		[JsonProperty("verified_reason")]
		public string VerifiedReason { get; private set; }

		/// <summary>
		/// The type of verification.
		/// </summary>
		[JsonProperty("verified_type")]
		public int VerifiedType { get; private set; }

		/// <summary>
		/// The type of verification, extended.
		/// </summary>
		[JsonProperty("verified_type_ext")]
		public int VerifiedTypeExt { get; private set; }
	}
}