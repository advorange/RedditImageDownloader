using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Lofter.Models
{
	/// <summary>
	/// Json model for a post from Lofter.
	/// </summary>
	public sealed class LofterPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		/// <remarks>The subdomain can be anything EXCEPT 'www' or nothing.</remarks>
		[JsonIgnore]
		public Uri PostUrl => new Uri($"http://a.lofter.com/post/{Permalink}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => NoteCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddMilliseconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <summary>
		/// The id of whoever created this post.
		/// </summary>
		[JsonProperty("blog_id")]
		public string BlogId { get; private set; }
		/// <summary>
		/// The day of the month this post was created.
		/// </summary>
		[JsonProperty("day_of_month")]
		public int DayOfMonth { get; private set; }
		/// <summary>
		/// The month this post was created.
		/// </summary>
		[JsonProperty("month")]
		public int Month { get; private set; }
		/// <summary>
		/// The year this post was created.
		/// </summary>
		[JsonProperty("year")]
		public int Year { get; private set; }
		/// <summary>
		/// The unix timestamp in milliseconds for when this post was created.
		/// </summary>
		[JsonProperty("time")]
		public long CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// How many notes this post has.
		/// </summary>
		[JsonProperty("note_count")]
		public int NoteCount { get; private set; }
		/// <summary>
		/// How many tags this post has.
		/// </summary>
		[JsonProperty("tag_count")]
		public int TagCount { get; private set; }
		/// <summary>
		/// Whether this post is a reblog.
		/// </summary>
		[JsonProperty("reblog")]
		public bool Reblog { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("cctype")]
		public string Cctype { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("valid")]
		public string Valid { get; private set; }
		/// <summary>
		/// The id of the post used for permalinking.
		/// </summary>
		[JsonProperty("permalink")]
		public string Permalink { get; private set; }
		/// <summary>
		/// The title of the post. This is truncated.
		/// </summary>
		[JsonProperty("notice_link_title")]
		public string NoticeLinkTitle { get; private set; }
		/// <summary>
		/// The url to the thumbnail.
		/// </summary>
		[JsonProperty("imgurl")]
		public string ThumbnailUrl { get; private set; }
		/// <summary>
		/// The images in the post.
		/// </summary>
		[JsonProperty("images")]
		public IList<LofterImage> Images { get; private set; }

		/// <summary>
		/// Fills in additional information in the post, such as images.
		/// </summary>
		/// <returns></returns>
		public async Task FillPost(IDownloaderClient client)
		{
			Images = new List<LofterImage>();

			var result = await client.GetHtmlAsync(() => client.GenerateReq(PostUrl)).CAF();
			var div = result.Value.DocumentNode.Descendants("div");
			var pics = div.Where(x => x.GetAttributeValue("class", null) == "pic").Select(x => x.Descendants("a").Single());
			foreach (var pic in pics)
			{
				var width = Convert.ToInt32(pic.GetAttributeValue("bigimgwidth", null));
				var height = Convert.ToInt32(pic.GetAttributeValue("bigimgheight", null));
				var url = pic.GetAttributeValue("bigimgsrc", null);
				Images.Add(new LofterImage(width, height, url));
			}
		}
		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromImages(Images.Select(x => x.FullImageUrl)));
		}
		/// <summary>
		/// Returns the post id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}