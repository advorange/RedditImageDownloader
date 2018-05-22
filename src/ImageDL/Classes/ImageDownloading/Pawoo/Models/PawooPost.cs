using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Json model for a post from Pawoo.
	/// </summary>
	public sealed class PawooPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonProperty("url")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => FavouritesCount;
		/// <inheritdoc />
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }
		/// <summary>
		/// The id of the post this is in reply to.
		/// </summary>
		[JsonProperty("in_reply_to_id")]
		public string InReplyToId { get; private set; }
		/// <summary>
		/// The id of the user this post is in reply to.
		/// </summary>
		[JsonProperty("in_reply_to_account_id")]
		public string InReplyToAccountId { get; private set; }
		/// <summary>
		/// Whether the post is NSFW.
		/// </summary>
		[JsonProperty("sensitive")]
		public bool Sensitive { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("spoiler_text")]
		public string SpoilerText { get; private set; }
		/// <summary>
		/// Public, private, etc.
		/// </summary>
		[JsonProperty("visibility")]
		public string Visibility { get; private set; }
		/// <summary>
		/// The language the post is in.
		/// </summary>
		[JsonProperty("language")]
		public string Language { get; private set; }
		/// <summary>
		/// Longer link, uses users/username
		/// </summary>
		[JsonProperty("uri")]
		public Uri Uri { get; private set; }
		/// <summary>
		/// The text and some other information in HTML.
		/// </summary>
		[JsonProperty("content")]
		public string Content { get; private set; }
		/// <summary>
		/// How many people have reblogged this.
		/// </summary>
		[JsonProperty("reblogs_count")]
		public int ReblogsCount { get; private set; }
		/// <summary>
		/// How many people have favorited this.
		/// </summary>
		[JsonProperty("favourites_count")]
		public int FavouritesCount { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("pixiv_cards")]
		public IList<object> PixivCards { get; private set; }
		/// <summary>
		/// Whether this post is pinned by the user.
		/// </summary>
		[JsonProperty("pinned")]
		public bool Pinned { get; private set; }
		/// <summary>
		/// Whether you have favorited this.
		/// </summary>
		[JsonProperty("favourited")]
		public bool Favourited { get; private set; }
		/// <summary>
		/// Whether you have reblogged this.
		/// </summary>
		[JsonProperty("reblogged")]
		public bool Reblogged { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("muted")]
		public bool Muted { get; private set; }
		/// <summary>
		/// The reblogged post.
		/// </summary>
		[JsonProperty("reblog")]
		public PawooPost Reblog { get; private set; }
		/// <summary>
		/// How this was accessed.
		/// </summary>
		[JsonProperty("application")]
		public PawooApplication Application { get; private set; }
		/// <summary>
		/// The user who posted this.
		/// </summary>
		[JsonProperty("account")]
		public PawooUser Account { get; private set; }
		/// <summary>
		/// The images of the post.
		/// </summary>
		[JsonProperty("media_attachments")]
		public IList<PawooMediaAttachment> MediaAttachments { get; private set; }
		/// <summary>
		/// Mentions of users.
		/// </summary>
		[JsonProperty("mentions")]
		public IList<PawooUserMention> Mentions { get; private set; }
		/// <summary>
		/// Tags in the post.
		/// </summary>
		[JsonProperty("tags")]
		public IList<PawooTag> Tags { get; private set; }
		/// <summary>
		/// Emojis in the post.
		/// </summary>
		[JsonProperty("emojis")]
		public IList<PawooEmoji> Emojis { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromImages(MediaAttachments.Select(x => x.Url)));
		}
	}
}