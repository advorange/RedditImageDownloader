using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Json model for a post from Twitter.
	/// </summary>
	public sealed class TwitterOAuthPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://twitter.com/i/web/status/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => FavoriteCount ?? 0;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				const string FORMAT = "ddd MMM dd HH:mm:ss zzz yyyy";
				if (DateTime.TryParse(CreatedAtString, out var dt))
				{
					return dt;
				}
				else if (DateTime.TryParseExact(CreatedAtString, FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				{
					return dt;
				}
				throw new ArgumentException($"Unable to convert {CreatedAtString} to a datetime.");
			}
		}
		/// <inheritdoc />
		[JsonIgnore]
		string IPost.Id => IdString;
		/// <summary>
		/// Time when this post was created at in UTC.
		/// </summary>
		[JsonProperty("created_at")]
		public string CreatedAtString { get; private set; }
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
		/// <summary>
		/// String of <see cref="Id"/>.
		/// </summary>
		[JsonProperty("id_str")]
		public string IdString { get; private set; }
		/// <summary>
		/// The text of the post.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
		/// <summary>
		/// What was used to send the post.
		/// </summary>
		[JsonProperty("source")]
		public string Source { get; private set; }
		/// <summary>
		/// Whether a tweet was shortened.
		/// </summary>
		[JsonProperty("truncated")]
		public bool Truncated { get; private set; }
		/// <summary>
		/// The id of the post this post is replying to.
		/// </summary>
		[JsonProperty("in_reply_to_status_id")]
		public long? InReplyToStatusId { get; private set; }
		/// <summary>
		/// String of <see cref="InReplyToStatusId"/>.
		/// </summary>
		[JsonProperty("in_reply_to_status_id_str")]
		public string InReplyToStatusIdStr { get; private set; }
		/// <summary>
		/// The id of the owner of the post this post is replying to.
		/// </summary>
		[JsonProperty("in_reply_to_user_id")]
		public long? InReplyToUserId { get; private set; }
		/// <summary>
		/// String of <see cref="InReplyToUserId"/>.
		/// </summary>
		[JsonProperty("in_reply_to_user_id_str")]
		public string InReplyToUserIdStr { get; private set; }
		/// <summary>
		/// The name of the owner of the post this post is replying to.
		/// </summary>
		[JsonProperty("in_reply_to_screen_name")]
		public string InReplyToScreenName { get; private set; }
		/// <summary>
		/// The person who posted this.
		/// </summary>
		[JsonProperty("user")]
		public TwitterOAuthUser User { get; private set; }
		/// <summary>
		/// The location of a post.
		/// </summary>
		[JsonProperty("coordinates")]
		public TwitterOAuthCoordinates? Coordinates { get; private set; }
		/// <summary>
		/// The location the post is associated with.
		/// </summary>
		[JsonProperty("place")]
		public TwitterOAuthPlace? Place { get; private set; }
		/// <summary>
		/// The id of the post quoted in this post.
		/// </summary>
		[JsonProperty("quoted_status_id")]
		public long QuoteStatusId { get; private set; }
		/// <summary>
		/// String of <see cref="QuoteStatusId"/>.
		/// </summary>
		[JsonProperty("quoted_status_id_str")]
		public string QuoteStatusIdStr { get; private set; }
		/// <summary>
		/// Whether this post is a quote.
		/// </summary>
		[JsonProperty("is_quote_status")]
		public bool IsQuoteStatus { get; private set; }
		/// <summary>
		/// The quoted post.
		/// </summary>
		[JsonProperty("quoted_status")]
		public TwitterOAuthPost QuotedStatus { get; private set; }
		/// <summary>
		/// The retweeted post.
		/// </summary>
		[JsonProperty("retweeted_status")]
		public TwitterOAuthPost RetweetedStatus { get; private set; }
		/// <summary>
		/// How many times this post has been quoted.
		/// </summary>
		[JsonProperty("quote_count")]
		public int? QuoteCount { get; private set; }
		/// <summary>
		/// How many times this post has been replied to.
		/// </summary>
		[JsonProperty("reply_count")]
		public int ReplyCount { get; private set; }
		/// <summary>
		/// How many times this post has been retweeted.
		/// </summary>
		[JsonProperty("retweet_count")]
		public int RetweetCount { get; private set; }
		/// <summary>
		/// How many times this post has been favorited.
		/// </summary>
		[JsonProperty("favorite_count")]
		public int? FavoriteCount { get; private set; }
		/// <summary>
		/// Information gotten from the text.
		/// </summary>
		[JsonProperty("entities")]
		public TwitterOAuthEntities Entities { get; private set; }
		/// <summary>
		/// Extra entities in a post. Contains all the images.
		/// </summary>
		[JsonProperty("extended_entities")]
		public TwitterOAuthEntities ExtendedEntities { get; private set; }
		/// <summary>
		/// Whether this post has been liked by you.
		/// </summary>
		[JsonProperty("favorited")]
		public bool? Favorited { get; private set; }
		/// <summary>
		/// Whether this post has been retweeted by you.
		/// </summary>
		[JsonProperty("retweeted")]
		public bool Retweeted { get; private set; }
		/// <summary>
		/// If the post contains a link.
		/// </summary>
		[JsonProperty("possibly_sensitive")]
		public bool? PossiblySensitive { get; private set; }
		/// <summary>
		/// Used when streaming tweets.
		/// </summary>
		[JsonProperty("filter_level")]
		public string FilterLevel { get; private set; }
		/// <summary>
		/// The language of the post.
		/// </summary>
		[JsonProperty("lang")]
		public string Language { get; private set; }
		/// <summary>
		/// Deprecated attribute, use <see cref="Coordinates"/> instead.
		/// </summary>
		[Obsolete]
		[JsonProperty("geo")]
		public object Geo { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			if (ExtendedEntities.Media == null)
			{
				return Task.FromResult(ImageResponse.FromNotFound(PostUrl));
			}
			var urls = ExtendedEntities.Media.Select(x => new Uri($"{x.MediaUrlHttps}:orig"));
			return Task.FromResult(ImageResponse.FromImages(urls));
		}
	}
}