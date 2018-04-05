using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	public class TumblrPost : IPost
	{
		[JsonProperty("url-with-slug")]
		public readonly string UrlWithSlug;
		[JsonProperty("type")]
		public readonly string Type;
		[JsonProperty("bookmarklet")]
		public readonly int Bookmarklet;
		[JsonProperty("mobile")]
		public readonly int Mobile;
		[JsonProperty("feed-item")]
		public readonly string FeedItem;
		[JsonProperty("from-feed-id")]
		public readonly int FromFeedId;
		[JsonProperty("format")]
		public readonly string Format;
		[JsonProperty("reblog-key")]
		public readonly string ReblogKey;
		[JsonProperty("slug")]
		public readonly string Slug;
		[JsonProperty("is-submission")]
		public readonly bool IsSubmission;
		[JsonProperty("like-button")]
		public readonly string LikeButton;
		[JsonProperty("reblog-button")]
		public readonly string ReblogButton;
		[JsonProperty("reblogged-from-url")]
		public readonly string RebloggedFromUrl;
		[JsonProperty("reblogged-from-name")]
		public readonly string RebloggedFromName;
		[JsonProperty("reblogged-from-title")]
		public readonly string RebloggedFromTitle;
		[JsonProperty("reblogged_from_avatar_url_16")]
		public readonly string RebloggedFromAvatarUrl16;
		[JsonProperty("reblogged_from_avatar_url_24")]
		public readonly string RebloggedFromAvatarUrl24;
		[JsonProperty("reblogged_from_avatar_url_30")]
		public readonly string RebloggedFromAvatarUrl30;
		[JsonProperty("reblogged_from_avatar_url_40")]
		public readonly string RebloggedFromAvatarUrl40;
		[JsonProperty("reblogged_from_avatar_url_48")]
		public readonly string RebloggedFromAvatarUrl48;
		[JsonProperty("reblogged_from_avatar_url_64")]
		public readonly string RebloggedFromAvatarUrl64;
		[JsonProperty("reblogged_from_avatar_url_96")]
		public readonly string RebloggedFromAvatarUrl96;
		[JsonProperty("reblogged_from_avatar_url_128")]
		public readonly string RebloggedFromAvatarUrl128;
		[JsonProperty("reblogged_from_avatar_url_512")]
		public readonly string RebloggedFromAvatarUrl512;
		[JsonProperty("reblogged-root-url")]
		public readonly string RebloggedRootUrl;
		[JsonProperty("reblogged-root-name")]
		public readonly string RebloggedRootName;
		[JsonProperty("reblogged-root-title")]
		public readonly string RebloggedRootTitle;
		[JsonProperty("reblogged_root_avatar_url_16")]
		public readonly string RebloggedRootAvatarUrl16;
		[JsonProperty("reblogged_root_avatar_url_24")]
		public readonly string RebloggedRootAvatarUrl24;
		[JsonProperty("reblogged_root_avatar_url_30")]
		public readonly string RebloggedRootAvatarUrl30;
		[JsonProperty("reblogged_root_avatar_url_40")]
		public readonly string RebloggedRootAvatarUrl40;
		[JsonProperty("reblogged_root_avatar_url_48")]
		public readonly string RebloggedRootAvatarUrl48;
		[JsonProperty("reblogged_root_avatar_url_64")]
		public readonly string RebloggedRootAvatarUrl64;
		[JsonProperty("reblogged_root_avatar_url_96")]
		public readonly string RebloggedRootAvatarUrl96;
		[JsonProperty("reblogged_root_avatar_url_128")]
		public readonly string RebloggedRootAvatarUrl128;
		[JsonProperty("reblogged_root_avatar_url_512")]
		public readonly string RebloggedRootAvatarUrl512;
		[JsonProperty("tumblelog")]
		public readonly Tumblelog2 Tumblelog;
		[JsonProperty("photo-caption")]
		public readonly string PhotoCaption;
		[JsonProperty("photo-link-url")]
		public readonly string PhotoLinkUrl;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("photo-url-1280")]
		public readonly string PhotoUrl1280;
		[JsonProperty("photo-url-500")]
		public readonly string PhotoUrl500;
		[JsonProperty("photo-url-400")]
		public readonly string PhotoUrl400;
		[JsonProperty("photo-url-250")]
		public readonly string PhotoUrl250;
		[JsonProperty("photo-url-100")]
		public readonly string PhotoUrl100;
		[JsonProperty("photo-url-75")]
		public readonly string PhotoUrl75;
		[JsonProperty("photos")]
		public readonly List<TumblrPhoto> Photos;

		[JsonProperty("id")]
		private readonly string _Id;
		[JsonProperty("url")]
		private readonly string _Url;
		[JsonProperty("date-gmt")]
		private readonly string _DateGmt;
		[JsonProperty("date")]
		private readonly string _Date;
		[JsonProperty("unix-timestamp")]
		private readonly int _UnixTimestamp;
		[JsonProperty("note-count")]
		private readonly int _NoteCount;

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public string PostUrl => _Url;
		/// <inheritdoc />
		public IEnumerable<string> ContentUrls => Photos?.Select(x => x.PhotoUrl1280) ?? new[] { PhotoUrl1280 };
		/// <inheritdoc />
		public int Score => _NoteCount;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(_UnixTimestamp)).ToUniversalTime();
	}
}
