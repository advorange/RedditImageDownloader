using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Json model for a Pixiv user.
	/// </summary>
	public sealed class PixivUser
	{
		/// <summary>
		/// The uesr's id.
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; private set; }
		/// <summary>
		/// The user's account name.
		/// </summary>
		[JsonProperty("account")]
		public string Account { get; private set; }
		/// <summary>
		/// the user's display name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// Whether you are following this person.
		/// </summary>
		[JsonProperty("is_following")]
		public bool? IsFollowing { get; private set; }
		/// <summary>
		/// Whether they are following you.
		/// </summary>
		[JsonProperty("is_follower")]
		public bool? IsFollower { get; private set; }
		/// <summary>
		/// Whether you are friends with them.
		/// </summary>
		[JsonProperty("is_friend")]
		public bool? IsFriend { get; private set; }
		/// <summary>
		/// Whether this person is a premium user.
		/// </summary>
		[JsonProperty("is_premium")]
		public bool? IsPremium { get; private set; }
		/// <summary>
		/// Urls of this person.
		/// </summary>
		[JsonProperty("profile_image_urls")]
		public IDictionary<string, Uri> ProfileImageUrls { get; private set; }
		/// <summary>
		/// The stats of the user.
		/// </summary>
		[JsonProperty("stats")]
		public PixivUserStats Stats { get; private set; }
		/// <summary>
		/// Misc info about the user.
		/// </summary>
		[JsonProperty("profile")]
		public PixivUserProfile Profile { get; private set; }
	}
}