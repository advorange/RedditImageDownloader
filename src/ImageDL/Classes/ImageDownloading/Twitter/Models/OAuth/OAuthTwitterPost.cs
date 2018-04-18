using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	public class UserMention
	{
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		[JsonProperty("screen_name")]
		public string ScreenName { get; private set; }
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		[JsonProperty("name")]
		public string Name { get; private set; }
		[JsonProperty("id")]
		public int Id { get; private set; }
	}
	public class Entities
	{
		[JsonProperty("user_mentions")]
		public IList<UserMention> UserMentions { get; private set; }
		[JsonProperty("urls")]
		public IList<object> Urls { get; private set; }
		[JsonProperty("hashtags")]
		public IList<object> Hashtags { get; private set; }
	}
	public class Hashtag
	{
		[JsonProperty("text")]
		public string Text { get; private set; }
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
	}
	public class Entities2
	{
		[JsonProperty("user_mentions")]
		public IList<object> UserMentions { get; private set; }
		[JsonProperty("urls")]
		public IList<object> Urls { get; private set; }
		[JsonProperty("hashtags")]
		public IList<Hashtag> Hashtags { get; private set; }
	}
	public class OAuthRetweetedStatus
	{
		[JsonProperty("text")]
		public string Text { get; private set; }
		[JsonProperty("truncated")]
		public bool Truncated { get; private set; }
		[JsonProperty("in_reply_to_user_id")]
		public object InReplyToUserId { get; private set; }
		[JsonProperty("in_reply_to_status_id")]
		public object InReplyToStatusId { get; private set; }
		[JsonProperty("favorited")]
		public bool Favorited { get; private set; }
		[JsonProperty("source")]
		public string Source { get; private set; }
		[JsonProperty("in_reply_to_screen_name")]
		public object InReplyToScreenName { get; private set; }
		[JsonProperty("in_reply_to_status_id_str")]
		public object InReplyToStatusIdStr { get; private set; }
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		[JsonProperty("entities")]
		public Entities2 Entities { get; private set; }
		[JsonProperty("contributors")]
		public object Contributors { get; private set; }
		[JsonProperty("retweeted")]
		public bool Retweeted { get; private set; }
		[JsonProperty("in_reply_to_user_id_str")]
		public object InReplyToUserIdStr { get; private set; }
		[JsonProperty("place")]
		public object Place { get; private set; }
		[JsonProperty("retweet_count")]
		public int RetweetCount { get; private set; }
		[JsonProperty("created_at")]
		public string CreatedAt { get; private set; }
		[JsonProperty("user")]
		public OAuthUser User { get; private set; }
		[JsonProperty("id")]
		public long Id { get; private set; }
		[JsonProperty("coordinates")]
		public object Coordinates { get; private set; }
		[JsonProperty("geo")]
		public object Geo { get; private set; }
	}
	public class OAuthUser
	{
		[JsonProperty("notifications")]
		public object Notifications { get; private set; }
		[JsonProperty("profile_use_background_image")]
		public bool ProfileUseBackgroundImage { get; private set; }
		[JsonProperty("statuses_count")]
		public int StatusesCount { get; private set; }
		[JsonProperty("profile_background_color")]
		public string ProfileBackgroundColor { get; private set; }
		[JsonProperty("followers_count")]
		public int FollowersCount { get; private set; }
		[JsonProperty("profile_image_url")]
		public string ProfileImageUrl { get; private set; }
		[JsonProperty("listed_count")]
		public int ListedCount { get; private set; }
		[JsonProperty("profile_background_image_url")]
		public string ProfileBackgroundImageUrl { get; private set; }
		[JsonProperty("description")]
		public string Description { get; private set; }
		[JsonProperty("screen_name")]
		public string ScreenName { get; private set; }
		[JsonProperty("default_profile")]
		public bool DefaultProfile { get; private set; }
		[JsonProperty("verified")]
		public bool Verified { get; private set; }
		[JsonProperty("time_zone")]
		public string TimeZone { get; private set; }
		[JsonProperty("profile_text_color")]
		public string ProfileTextColor { get; private set; }
		[JsonProperty("is_translator")]
		public bool IsTranslator { get; private set; }
		[JsonProperty("profile_sidebar_fill_color")]
		public string ProfileSidebarFillColor { get; private set; }
		[JsonProperty("location")]
		public string Location { get; private set; }
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		[JsonProperty("default_profile_image")]
		public bool DefaultProfileImage { get; private set; }
		[JsonProperty("profile_background_tile")]
		public bool ProfileBackgroundTile { get; private set; }
		[JsonProperty("lang")]
		public string Lang { get; private set; }
		[JsonProperty("friends_count")]
		public int FriendsCount { get; private set; }
		[JsonProperty("protected")]
		public bool Protected { get; private set; }
		[JsonProperty("favourites_count")]
		public int FavouritesCount { get; private set; }
		[JsonProperty("created_at")]
		public string CreatedAt { get; private set; }
		[JsonProperty("profile_link_color")]
		public string ProfileLinkColor { get; private set; }
		[JsonProperty("name")]
		public string Name { get; private set; }
		[JsonProperty("show_all_inline_media")]
		public bool ShowAllInlineMedia { get; private set; }
		[JsonProperty("follow_request_sent")]
		public object FollowRequestSent { get; private set; }
		[JsonProperty("geo_enabled")]
		public bool GeoEnabled { get; private set; }
		[JsonProperty("profile_sidebar_border_color")]
		public string ProfileSidebarBorderColor { get; private set; }
		[JsonProperty("url")]
		public object Url { get; private set; }
		[JsonProperty("id")]
		public int Id { get; private set; }
		[JsonProperty("contributors_enabled")]
		public bool ContributorsEnabled { get; private set; }
		[JsonProperty("following")]
		public object Following { get; private set; }
		[JsonProperty("utc_offset")]
		public int UtcOffset { get; private set; }
	}
	public class OAuthTwitterPost
	{
		[JsonProperty("text")]
		public string Text { get; private set; }
		[JsonProperty("truncated")]
		public bool Truncated { get; private set; }
		[JsonProperty("in_reply_to_user_id")]
		public object InReplyToUserId { get; private set; }
		[JsonProperty("in_reply_to_status_id")]
		public object InReplyToStatusId { get; private set; }
		[JsonProperty("favorited")]
		public bool Favorited { get; private set; }
		[JsonProperty("source")]
		public string Source { get; private set; }
		[JsonProperty("in_reply_to_screen_name")]
		public object InReplyToScreenName { get; private set; }
		[JsonProperty("in_reply_to_status_id_str")]
		public object InReplyToStatusIdStr { get; private set; }
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		[JsonProperty("entities")]
		public Entities Entities { get; private set; }
		[JsonProperty("contributors")]
		public object Contributors { get; private set; }
		[JsonProperty("retweeted")]
		public bool Retweeted { get; private set; }
		[JsonProperty("in_reply_to_user_id_str")]
		public object InReplyToUserIdStr { get; private set; }
		[JsonProperty("place")]
		public object Place { get; private set; }
		[JsonProperty("retweet_count")]
		public int RetweetCount { get; private set; }
		[JsonProperty("created_at")]
		public string CreatedAt { get; private set; }
		[JsonProperty("retweeted_status")]
		public OAuthRetweetedStatus RetweetedStatus { get; private set; }
		[JsonProperty("user")]
		public OAuthUser User { get; private set; }
		[JsonProperty("id")]
		public long Id { get; private set; }
		[JsonProperty("coordinates")]
		public object Coordinates { get; private set; }
		[JsonProperty("geo")]
		public object Geo { get; private set; }
	}
}