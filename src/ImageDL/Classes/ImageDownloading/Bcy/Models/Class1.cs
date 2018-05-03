using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	public class BcyFirstImage : ISize
	{
		[JsonProperty("path")]
		public string Path { get; private set; }
		[JsonProperty("w")]
		public int Width { get; private set; }
		[JsonProperty("h")]
		public int Height { get; private set; }
	}
	public class BcyWork
	{
		[JsonProperty("text")]
		public string Text { get; private set; }
		[JsonProperty("link")]
		public string Link { get; private set; }
	}
	public class BcyPostTag
	{
		[JsonProperty("tag_id")]
		public string TagId { get; private set; }
		[JsonProperty("tag_name")]
		public string TagName { get; private set; }
	}

	public struct BcyDetails
	{
		[JsonProperty("rp_id")]
		public int RpId { get; private set; }
		[JsonProperty("title")]
		public string Title { get; private set; }
		[JsonProperty("type")]
		public string Type { get; private set; }
		[JsonProperty("reply_count")]
		public int ReplyCount { get; private set; }
		[JsonProperty("dp_id")]
		public string DpId { get; private set; }
		[JsonProperty("uid")]
		public string Uid { get; private set; }
		[JsonProperty("img_src")]
		public string ImgSrc { get; private set; }
		[JsonProperty("first_image")]
		public BcyFirstImage FirstImage { get; private set; }
		[JsonProperty("pic_num")]
		public int PicNum { get; private set; }
		[JsonProperty("work")]
		public BcyWork Work { get; private set; }
		[JsonProperty("plain")]
		public string Plain { get; private set; }
		[JsonProperty("post_locked")]
		public bool PostLocked { get; private set; }
		[JsonProperty("have_ding")]
		public bool HaveDing { get; private set; }
		[JsonProperty("ding_num")]
		public int DingNum { get; private set; }
		[JsonProperty("view_need_login")]
		public bool ViewNeedLogin { get; private set; }
		[JsonProperty("view_need_fans")]
		public bool ViewNeedFans { get; private set; }
		[JsonProperty("post_tags")]
		public IList<BcyPostTag> PostTags { get; private set; }
		[JsonProperty("followstate")]
		public string Followstate { get; private set; }
	}
	/// <summary>
	/// Json model for a post from Bcy.
	/// </summary>
	public sealed class BcyPost : IPost
	{
		/// <summary>
		/// Not sure, not post id or submitter id though.
		/// </summary>
		[JsonProperty("tl_id")]
		public int TlId { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when this was created.
		/// </summary>
		[JsonProperty("ctime")]
		public long CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The id of the submitter.
		/// </summary>
		[JsonProperty("uid")]
		public int UserId { get; private set; }
		/// <summary>
		/// Same as <see cref="UserId"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("ouid")]
		public int OUserId { get; private set; }
		/// <summary>
		/// Not sure, usually is "drawer."
		/// </summary>
		[JsonProperty("otype")]
		public string OType { get; private set; }
		/// <summary>
		/// Not sure, usually is "post."
		/// </summary>
		[JsonProperty("otype_data")]
		public string OTypeData { get; private set; }
		/// <summary>
		/// Not sure, usually is 0.
		/// </summary>
		[JsonProperty("trans_id")]
		public int TransId { get; private set; }
		/// <summary>
		/// Details about the post, such as images.
		/// </summary>
		[JsonProperty("detail")]
		public BcyDetails Detail { get; private set; }
		/// <summary>
		/// The submitter's avatar url.
		/// </summary>
		[JsonProperty("avatar")]
		public string UserAvatar { get; private set; }
		/// <summary>
		/// The submitter's name.
		/// </summary>
		[JsonProperty("uname")]
		public string Username { get; private set; }
		/// <summary>
		/// Same as <see cref="UserAvatar"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("oavatar")]
		public string OUserAvatar { get; private set; }
		/// <summary>
		/// Same as <see cref="Username"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("ouname")]
		public string OUsername { get; private set; }

		public string Id => throw new NotImplementedException();

		public Uri PostUrl => throw new NotImplementedException();

		public int Score => throw new NotImplementedException();

		public DateTime CreatedAt => throw new NotImplementedException();

		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			throw new NotImplementedException();
		}
	}
}