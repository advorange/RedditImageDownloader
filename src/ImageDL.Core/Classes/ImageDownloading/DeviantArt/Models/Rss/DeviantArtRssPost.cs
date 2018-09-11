using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// Json model for a DeviantArt post gotten via an rss feed.
	/// </summary>
	public sealed class DeviantArtRssPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => PostUrl.ToString().Split('-', '/').Last().Split('.').First();
		/// <inheritdoc />
		[JsonProperty("link")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => -1;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				if (DateTime.TryParse(CreatedAtString, out var dt))
				{
					return dt;
				}
				var parts = CreatedAtString.Split(' ');
				//DateTime.TryParse only works with GMT as the timezone
				parts[parts.Length - 1] = "GMT";
				if (DateTime.TryParse(String.Join(" ", parts), out dt))
				{
					return dt; //Will be off by a few hours, but whatever
				}
				throw new ArgumentException($"Unable to convert {CreatedAtString} to a datetime.");
			}
		}
		/// <summary>
		/// String representation of when the post was created at.
		/// </summary>
		[JsonProperty("pubDate")]
		public string CreatedAtString { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// A link to the post.
		/// </summary>
		[JsonProperty("guid")]
		public DeviantArtRssGuid Guid { get; private set; }
		/// <summary>
		/// The title again.
		/// </summary>
		[JsonProperty("media:title")]
		public DeviantArtRssTitle MediaTitle { get; private set; }
		/// <summary>
		/// Keywords associated with the post.
		/// </summary>
		[JsonProperty("media:keywords")]
		public string MediaKeywords { get; private set; }
		/// <summary>
		/// The rating of the post, e.g. nonadult.
		/// </summary>
		[JsonProperty("media:rating")]
		public string MediaRating { get; private set; }
		/// <summary>
		/// The category this post was posted in.
		/// </summary>
		[JsonProperty("media:category")]
		public DeviantArtRssCategory MediaCategory { get; private set; }
		/// <summary>
		/// Who made this post.
		/// </summary>
		[JsonProperty("media:credit")]
		public IList<DeviantArtRssCredit> MediaCredit { get; private set; }
		/// <summary>
		/// Who owns this post.
		/// </summary>
		[JsonProperty("media:copyright")]
		public DeviantArtRssCopyright MediaCopyright { get; private set; }
		/// <summary>
		/// Html description.
		/// </summary>
		[JsonProperty("media:description")]
		public DeviantArtRssDescription MediaDescription { get; private set; }
		/// <summary>
		/// Various thumbnails for the image.
		/// </summary>
		[JsonProperty("media:thumbnail")]
		public IList<DeviantArtRssThumbnail> MediaThumbnail { get; private set; }
		/// <summary>
		/// The direct link and size of the image.
		/// </summary>
		[JsonProperty("media:content")]
		public DeviantArtRssContent MediaContent { get; private set; }
		/// <summary>
		/// Html description again.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromUrl(MediaContent.Url));
	}
}