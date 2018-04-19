using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// A Twitter user.
	/// </summary>
	public struct TwitterOAuthUser
	{
		/// <summary>
		/// The id of the user.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
		/// <summary>
		/// String of <see cref="Id"/>.
		/// </summary>
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		/// <summary>
		/// The name of the user.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The display name.
		/// </summary>
		[JsonProperty("screen_name")]
		public string ScreenName { get; private set; }
		/// <summary>
		/// User defined location.
		/// </summary>
		[JsonProperty("location")]
		public string Location { get; private set; }
		/// <summary>
		/// User defined url.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; private set; }
		/// <summary>
		/// User defined description.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// Whether the account is private.
		/// </summary>
		[JsonProperty("protected")]
		public bool Protected { get; private set; }
		/// <summary>
		/// Whether the account is verified.
		/// </summary>
		[JsonProperty("verified")]
		public bool Verified { get; private set; }
		/// <summary>
		/// How many followers the account has.
		/// </summary>
		[JsonProperty("followers_count")]
		public int FollowersCount { get; private set; }
		/// <summary>
		/// How many friends the account has.
		/// </summary>
		[JsonProperty("friends_count")]
		public int FriendsCount { get; private set; }
		/// <summary>
		/// How many lists the account is in.
		/// </summary>
		[JsonProperty("listed_count")]
		public int ListedCount { get; private set; }
		/// <summary>
		/// How many favorites the account has.
		/// </summary>
		[JsonProperty("favourites_count")]
		public int FavoritesCount { get; private set; }
		/// <summary>
		/// How many tweets and retweets the account has.
		/// </summary>
		[JsonProperty("statuses_count")]
		public int StatusesCount { get; private set; }
		/// <summary>
		/// When the account was created in UTC.
		/// </summary>
		[JsonProperty("created_at")]
		public string CreatedAt { get; private set; }
		/// <summary>
		/// The offset from UTC in seconds.
		/// </summary>
		[JsonProperty("utc_offset")]
		public int? UtcOffset { get; private set; }
		/// <summary>
		/// The timezone the account is in.
		/// </summary>
		[JsonProperty("time_zone")]
		public string TimeZone { get; private set; }
		/// <summary>
		/// Whether tweets will be geotagged.
		/// </summary>
		[JsonProperty("geo_enabled")]
		public bool GeoEnabled { get; private set; }
		/// <summary>
		/// The language of the account.
		/// </summary>
		[JsonProperty("lang")]
		public string Lang { get; private set; }
		/// <summary>
		/// Whether the account has multiple people accessing it through a special Twitter method.
		/// </summary>
		[JsonProperty("contributors_enabled")]
		public bool ContributorsEnabled { get; private set; }
		/// <summary>
		/// Background color in hex.
		/// </summary>
		[JsonProperty("profile_background_color")]
		public string ProfileBackgroundColor { get; private set; }
		/// <summary>
		/// Http url to background image.
		/// </summary>
		[JsonProperty("profile_background_image_url")]
		public string ProfileBackgroundImageUrl { get; private set; }
		/// <summary>
		/// Https url to background image.
		/// </summary>
		[JsonProperty("profile_background_image_url_https")]
		public string ProfileBackgroundImageUrlHttps { get; private set; }
		/// <summary>
		/// Whether the background image url should be displayed.
		/// </summary>
		[JsonProperty("profile_background_tile")]
		public bool ProfileBackgroundTile { get; private set; }
		/// <summary>
		/// Https url to banner.
		/// </summary>
		[JsonProperty("profile_banner_url")]
		public string ProfileBannerUrl { get; private set; }
		/// <summary>
		/// Http url to image.
		/// </summary>
		[JsonProperty("profile_image_url")]
		public string ProfileImageUrl { get; private set; }
		/// <summary>
		/// Https url to image.
		/// </summary>
		[JsonProperty("profile_image_url_https")]
		public string ProfileImageUrlHttps { get; private set; }
		/// <summary>
		/// Link color in hex.
		/// </summary>
		[JsonProperty("profile_link_color")]
		public string ProfileLinkColor { get; private set; }
		/// <summary>
		/// Sidebar border color in hex.
		/// </summary>
		[JsonProperty("profile_sidebar_border_color")]
		public string ProfileSidebarBorderColor { get; private set; }
		/// <summary>
		/// Sidebar fill color in hex.
		/// </summary>
		[JsonProperty("profile_sidebar_fill_color")]
		public string ProfileSidebarFillColor { get; private set; }
		/// <summary>
		/// Text color in hex.
		/// </summary>
		[JsonProperty("profile_text_color")]
		public string ProfileTextColor { get; private set; }
		/// <summary>
		/// Whether to show the background image.
		/// </summary>
		[JsonProperty("profile_use_background_image")]
		public bool ProfileUseBackgroundImage { get; private set; }
		/// <summary>
		/// Whether the account has not edited their theme.
		/// </summary>
		[JsonProperty("default_profile")]
		public bool DefaultProfile { get; private set; }
		/// <summary>
		/// Whether the account has not uploaded a profile picture.
		/// </summary>
		[JsonProperty("default_profile_image")]
		public bool DefaultProfileImage { get; private set; }
		/// <summary>
		/// Countries the account is withheld from.
		/// </summary>
		[JsonProperty("withheld_in_countries")]
		public string WithheldInCountries { get; private set; }
		/// <summary>
		/// Content being withheld.
		/// </summary>
		[JsonProperty("withheld_scope")]
		public string WithheldScope { get; private set; }
		/// <summary>
		/// Whether the user was a translator.
		/// </summary>
		[Obsolete]
		[JsonProperty("is_translator")]
		public bool IsTranslator { get; private set; }
		/// <summary>
		/// Whether you are following the user.
		/// </summary>
		[Obsolete]
		[JsonProperty("following")]
		public bool? Following { get; private set; }
		/// <summary>
		/// Whether you are receiving notifications from the user.
		/// </summary>
		[Obsolete]
		[JsonProperty("notifications")]
		public bool? Notifications { get; private set; }
	}
}