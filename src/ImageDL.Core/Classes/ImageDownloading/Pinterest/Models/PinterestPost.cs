using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Json model for a Pinterest post.
	/// </summary>
	public class PinterestPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.pinterest.com/pin/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => LikeCount;
		/// <inheritdoc />
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }
		/// <summary>
		/// The source domain.
		/// </summary>
		[JsonProperty("domain")]
		public string Domain { get; private set; }
		/// <summary>
		/// Used for search engines tracking.
		/// </summary>
		[JsonProperty("tracking_params")]
		public string TrackingParams { get; private set; }
		/// <summary>
		/// Holds how many times this was downloaded.
		/// </summary>
		[JsonProperty("aggregated_pin_data")]
		public PinterestAggregatePostData AggregatedPinData { get; private set; }
		/// <summary>
		/// Hash of the image.
		/// </summary>
		[JsonProperty("image_signature")]
		public string ImageSignature { get; private set; }
		/// <summary>
		/// How many likes the post has.
		/// </summary>
		[JsonProperty("like_count")]
		public int LikeCount { get; private set; }
		/// <summary>
		/// Various sizes of the image in the post.
		/// </summary>
		[JsonProperty("images")]
		public IDictionary<string, PinterestImage> Images { get; private set; }
		/// <summary>
		/// The largest version of the image.
		/// </summary>
		[JsonIgnore]
		public PinterestImage LargestImage => Images.Values.OrderBy(x => x.Width).ThenBy(x => x.Height).Last();
		/// <summary>
		/// The description in Html format.
		/// </summary>
		[JsonProperty("description_html")]
		public string DescriptionHtml { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("jsonld_collection_count_pin_group")]
		public object JsonldCollectionCountPinGroup { get; private set; }
		/// <summary>
		/// How many comments a post has.
		/// </summary>
		[JsonProperty("comment_count")]
		public int CommentCount { get; private set; }
		/// <summary>
		/// The board the post is on.
		/// </summary>
		[JsonProperty("board")]
		public PinterestBoard Board { get; private set; }
		/// <summary>
		/// The type of object, e.g. pin.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("attribution")]
		public object Attribution { get; private set; }
		/// <summary>
		/// The description in non Html format.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// Various metadata about a post.
		/// </summary>
		[JsonProperty("rich_metadata")]
		public PinterestRichMetadata RichMetadata { get; private set; }
		/// <summary>
		/// The url to the source.
		/// </summary>
		[JsonProperty("link")]
		public Uri Link { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("has_required_attribution_provider")]
		public bool HasRequiredAttributionProvider { get; private set; }
		/// <summary>
		/// The user who posted this.
		/// </summary>
		[JsonProperty("pinner")]
		public PinterestUser Pinner { get; private set; }
		/// <summary>
		/// How many repins this has.
		/// </summary>
		[JsonProperty("repin_count")]
		public int RepinCount { get; private set; }
		/// <summary>
		/// The color most common in the image.
		/// </summary>
		[JsonProperty("dominant_color")]
		public string DominantColor { get; private set; }
		/// <summary>
		/// Various info about localization and tags.
		/// </summary>
		[JsonProperty("pin_join")]
		public PinterestPinJoin PinJoin { get; private set; }
		/// <summary>
		/// Extra information from posting to a site from Pinterest.
		/// </summary>
		[JsonProperty("rich_summary")]
		public PinterestRichSummary RichSummary { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(LargestImage.Url));
		}
		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({LargestImage.Width}x{LargestImage.Height})";
		}
	}
}