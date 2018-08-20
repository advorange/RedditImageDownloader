using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Json model for a post from Pixiv.
	/// </summary>
	public sealed class PixivPost : IPost, ISize
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Stats.ScoredCount;
		/// <inheritdoc />
		[JsonProperty("created_time")]
		public DateTime CreatedAt { get; private set; }
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The caption of the post.
		/// </summary>
		[JsonProperty("caption")]
		public string Caption { get; private set; }
		/// <summary>
		/// The tags of this post.
		/// </summary>
		[JsonProperty("tags")]
		public IList<string> Tags { get; private set; }
		/// <summary>
		/// The programs/objects used to make this post.
		/// </summary>
		[JsonProperty("tools")]
		public IList<string> Tools { get; private set; }
		/// <summary>
		/// The type of url paired with its url.
		/// </summary>
		[JsonProperty("image_urls")]
		public IDictionary<string, Uri> ImageUrls { get; private set; }
		/// <summary>
		/// The stats of the post.
		/// </summary>
		[JsonProperty("stats")]
		public PixivStats Stats { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("publicity")]
		public int Publicity { get; private set; }
		/// <summary>
		/// The age limit on the post, e.g. all-age, r18, etc.
		/// </summary>
		[JsonProperty("age_limit")]
		public string AgeLimit { get; private set; }
		/// <summary>
		/// When the post was reuploaded.
		/// </summary>
		[JsonProperty("reuploaded_time")]
		public DateTime ReuploadedAt { get; private set; }
		/// <summary>
		/// The user who uploaded the post.
		/// </summary>
		[JsonProperty("user")]
		public PixivUser User { get; private set; }
		/// <summary>
		/// Whether there is more than one image.
		/// </summary>
		[JsonProperty("is_manga")]
		public bool IsManga { get; private set; }
		/// <summary>
		/// Whether you like it.
		/// </summary>
		[JsonProperty("is_liked")]
		public bool IsLiked { get; private set; }
		/// <summary>
		/// The id of your favorite on the post.
		/// </summary>
		[JsonProperty("favorite_id")]
		public int FavoriteId { get; private set; }
		/// <summary>
		/// How many images are in the post.
		/// </summary>
		[JsonProperty("page_count")]
		public int PageCount { get; private set; }
		/// <summary>
		/// The book style, e.g. none, etc.
		/// </summary>
		[JsonProperty("book_style")]
		public string BookStyle { get; private set; }
		/// <summary>
		/// The type of post, e.g. illustration, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The urls of the images in the post.
		/// </summary>
		[JsonProperty("metadata")]
		public PixivPostMetadata Metadata { get; private set; }
		/// <summary>
		/// Content types and whether this post has them.
		/// </summary>
		[JsonProperty("content_type")]
		public IDictionary<string, bool> ContentType { get; private set; }
		/// <summary>
		/// Sanity level, e.g. semi_black, etc.
		/// </summary>
		[JsonProperty("sanity_level")]
		public string SanityLevel { get; private set; }

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			//Only one image, so return the one image's url
			if (PageCount == 1)
			{
				return ImageResponse.FromUrl(ImageUrls["large"]);
			}
			//Metadata is null so we need to get it again
			else if (Metadata == null)
			{
				var query = new Uri($"https://public-api.secure.pixiv.net/v1/works/{Id}.json" +
					$"?access_token={client.ApiKeys[typeof(PixivPostDownloader)]}" +
					$"&include_stats=1" +
					$"&include_sanity_level=1" +
					$"&image_sizes=large" +
					$"&inclue_metadata=1" +
					$"&include_content_type=1");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					throw new InvalidOperationException("Unable to use the Pixiv api.");
				}
				//First b/c returns a list
				Metadata = JObject.Parse(result.Value)["response"].First["metadata"].ToObject<PixivPostMetadata>();
			}
			return ImageResponse.FromImages(Metadata.Pages.Select(x => x.ImageUrls["large"]));
		}
		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}