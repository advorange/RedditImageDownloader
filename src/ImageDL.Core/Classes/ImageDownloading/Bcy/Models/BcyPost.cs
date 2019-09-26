using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Json model for a Bcy post.
	/// </summary>
	public sealed class BcyPost : IPost
	{
		/// <summary>
		/// Link to the user that posted this' avatar.
		/// </summary>
		[JsonProperty("avatar")]
		public string Avatar { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();

		/// <summary>
		/// The unix timestamp in seconds.
		/// </summary>
		[JsonProperty("ctime")]
		public int CreatedAtTimestamp { get; private set; }

		/// <inheritdoc />
		[JsonProperty("item_id")]
		public string Id { get; private set; }

		/// <summary>
		/// How many likes the post has.
		/// </summary>
		[JsonProperty("like_count")]
		public int LikeCount { get; private set; }

		/// <summary>
		/// Thumbnails, up to 3.
		/// </summary>
		[JsonProperty("multi")]
		public IList<BcyMulti> Multi { get; private set; }

		/// <summary>
		/// How many images are in the post.
		/// </summary>
		[JsonProperty("pic_num")]
		public int PicNum { get; private set; }

		/// <summary>
		/// Description of thepost.
		/// </summary>
		[JsonProperty("plain")]
		public string Plain { get; private set; }

		/// <summary>
		/// The tags on the post.
		/// </summary>
		[JsonProperty("post_tags")]
		public IList<BcyPostTag> PostTags { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://bcy.net/item/detail/{Id}");

		/// <summary>
		/// The source of the character again.
		/// </summary>
		[JsonProperty("real_name")]
		public string RealName { get; private set; }

		/// <summary>
		/// How many replies the post has.
		/// </summary>
		[JsonProperty("reply_count")]
		public int ReplyCount { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public int Score => LikeCount;

		/// <summary>
		/// How many shares the post has.
		/// </summary>
		[JsonProperty("share_count")]
		public int ShareCount { get; private set; }

		/// <summary>
		/// The type of object, e.g. note.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// The name of the user that posted this.
		/// </summary>
		[JsonProperty("uname")]
		public string Uname { get; private set; }

		/// <summary>
		/// The id of the user that posted this.
		/// </summary>
		[JsonProperty("uid")]
		public string UserId { get; private set; }

		/// <summary>
		/// Whether you have liked the post.
		/// </summary>
		[JsonProperty("user_liked")]
		public bool UserLiked { get; private set; }

		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("wid")]
		public int Wid { get; private set; }

		/// <summary>
		/// The source of the character.
		/// </summary>
		[JsonProperty("work")]
		public string Work { get; private set; }

		/// <summary>
		/// Thumbnail for the source.
		/// </summary>
		[JsonProperty("work_cover")]
		public string WorkCover { get; private set; }

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => await BcyPostDownloader.GetBcyImagesAsync(client, PostUrl).CAF();

		/// <summary>
		/// Returns the post id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}