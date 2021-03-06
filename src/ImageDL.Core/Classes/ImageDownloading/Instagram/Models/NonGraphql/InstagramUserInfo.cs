﻿using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information relating to the user.
	/// </summary>
	public sealed class InstagramUserInfo
	{
		/// <summary>
		/// The posts a user has made.
		/// </summary>
		[JsonProperty("edge_owner_to_timeline_media")]
		public InstagramMediaTimeline Content { get; private set; }
	}
}