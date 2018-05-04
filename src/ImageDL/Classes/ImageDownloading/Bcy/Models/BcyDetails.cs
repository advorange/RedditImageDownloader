using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Details about a Bcy post.
	/// </summary>
	public struct BcyDetails
	{
		/// <summary>
		/// The id of the post. This is unique.
		/// </summary>
		[JsonIgnore]
		public int PostId => _RpId ?? _UdId ?? _UaId ?? _PostId ?? -1;
		/// <summary>
		/// The name of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The type of illustration this is.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// How many comments this has.
		/// </summary>
		[JsonProperty("reply_count")]
		public int ReplyCount { get; private set; }
		/// <summary>
		/// The id of the channel or group this was posted in.
		/// </summary>
		[JsonIgnore]
		public int ChannelId => _CpId ?? _DpId ?? _GId ?? -1;
		/// <summary>
		/// The id of the user that posted this.
		/// </summary>
		[JsonProperty("uid")]
		public int UserId { get; private set; }
		/// <summary>
		/// The url to the first thumbnail.
		/// </summary>
		[JsonProperty("img_src")]
		public string ThumbnailSource { get; private set; }
		/// <summary>
		/// The first image in the post.
		/// </summary>
		[JsonProperty("first_image")]
		public BcyFirstImage FirstImage { get; private set; }
		/// <summary>
		/// How many pictures the post has.
		/// </summary>
		[JsonProperty("pic_num")]
		public int PictureCount { get; private set; }
		/// <summary>
		/// The source of the characters.
		/// </summary>
		[JsonProperty("work")]
		public BcyWork Work { get; private set; }
		/// <summary>
		/// Not sure, only shows up on cosplay posts.
		/// </summary>
		[JsonProperty("character")]
		public string Character { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("plain")]
		public string Plain { get; private set; }
		/// <summary>
		/// Whether the post is locked.
		/// </summary>
		[JsonProperty("post_locked")]
		public bool PostLocked { get; private set; }
		/// <summary>
		/// Whether you have favorited the post.
		/// </summary>
		[JsonProperty("have_ding")]
		public bool HaveFavorited { get; private set; }
		/// <summary>
		/// How many favorites the post has.
		/// </summary>
		[JsonProperty("ding_num")]
		public int FavoriteCount { get; private set; }
		/// <summary>
		/// Whether you need to be logged in to see the post.
		/// </summary>
		[JsonProperty("view_need_login")]
		public bool ViewNeedLogin { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("view_need_fans")]
		public bool ViewNeedFans { get; private set; }
		/// <summary>
		/// The tags on the post.
		/// </summary>
		[JsonProperty("post_tags")]
		public IList<BcyPostTag> PostTags { get; private set; }
		/// <summary>
		/// Whether you are following the user or not.
		/// </summary>
		[JsonProperty("followstate")]
		public string Followstate { get; private set; }
		/// <summary>
		/// The post id if this is an image.
		/// </summary>
		[JsonProperty("rp_id")]
		private int? _RpId { get; set; }
		/// <summary>
		/// Only shows up on dailies.
		/// </summary>
		[JsonProperty("ud_id")]
		private int? _UdId { get; set; }
		/// <summary>
		/// Only shows up in asks.
		/// </summary>
		[JsonProperty("ua_id")]
		private int? _UaId { get; set; }
		/// <summary>
		/// The post id if this is a group.
		/// </summary>
		[JsonProperty("post_id")]
		private int? _PostId { get; set; }
		/// <summary>
		/// Only shows up on cosplay posts.
		/// </summary>
		[JsonProperty("cp_id")]
		private int? _CpId { get; set; }
		/// <summary>
		/// Only shows up on drawings.
		/// </summary>
		[JsonProperty("dp_id")]
		private int? _DpId { get; set; }
		/// <summary>
		/// Only shows up on groups.
		/// </summary>
		[JsonProperty("gid")]
		private int? _GId { get; set; }

		/// <summary>
		/// Returns the post id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return PostId.ToString();
		}
	}
}