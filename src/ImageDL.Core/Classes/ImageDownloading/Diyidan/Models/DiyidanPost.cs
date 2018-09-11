using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Diyidan.Models
{
	/// <summary>
	/// Model for a Diyidan post.
	/// </summary>
    public sealed class DiyidanPost : IPost
    {
		/// <inheritdoc />
		[JsonProperty("post_id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.diyidan.com/main/post/{Id}/detail/1");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => LikeCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				//Get date posted based off of thumbnail url, can't get more accurate though
				//E.G: //image.diyidan.net/post/2018/5/20/VuQFHKpxqiSHjWFZ.jpg!webindex
				var parts = ThumbnailUrl.ToString().Split(new[] { "/post/", "/shortvideo/", "/video/" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('/');
				var date = parts.Take(3).Select(x => Convert.ToInt32(x)).ToArray();
				return new DateTime(date[0], date[1], date[2]);
			}
		}
		/// <summary>
		/// How many likes the post has.
		/// </summary>
		[JsonProperty("like_count")]
		public int LikeCount { get; private set; }
		/// <summary>
		/// How many comments the post has.
		/// </summary>
		[JsonProperty("comment_count")]
		public int CommentCount { get; private set; }
		/// <summary>
		/// How many people have added the post to a collection.
		/// </summary>
		[JsonProperty("collect_count")]
		public int CollectCount { get; private set; }
		/// <summary>
		/// The link to the image used for the post.
		/// </summary>
		[JsonProperty("thumbnail_url")]
		public Uri ThumbnailUrl { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The description of the post.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// The tags of the image.
		/// </summary>
		[JsonProperty("tags")]
		public IList<string> Tags { get; private set; }
		/// <summary>
		/// The url to the author's profile picture.
		/// </summary>
		[JsonProperty("author_icon_url")]
		public Uri AuthorIconUrl { get; private set; }
		/// <summary>
		/// The author's username.
		/// </summary>
		[JsonProperty("author_username")]
		public string AuthorUsername { get; private set; }
		/// <summary>
		/// The author'd id.
		/// </summary>
		[JsonProperty("author_id")]
		public string AuthorId { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="DiyidanPost"/>.
		/// </summary>
		/// <param name="node">The element containing post information.</param>
		public DiyidanPost(HtmlNode node)
		{
			var span = node.Descendants("span");
			var div = node.Descendants("div");

			Id = Convert.ToUInt64(node.GetAttributeValue("data-post_id", null).Split('_').Last()).ToString(); //Make sure valid number

			LikeCount = Convert.ToInt32(span.Single(x => x.GetAttributeValue("class", null) == "ie01").Descendants("p").Single().InnerText);
			CommentCount = Convert.ToInt32(span.Single(x => x.GetAttributeValue("class", null) == "ie02").Descendants("p").Single().InnerText);
			CollectCount = Convert.ToInt32(span.Single(x => x.GetAttributeValue("class", null) == "ie03").Descendants("p").Single().InnerText);

			var thumbnail = div.Single(x => x.GetAttributeValue("class", null) == "sheng_img").Descendants("img").Single();
			//E.G: //image.diyidan.net/post/2018/5/20/VuQFHKpxqiSHjWFZ.jpg!webindex
			ThumbnailUrl = new Uri($"https:{thumbnail.GetAttributeValue("src", null)}");

			var info = div.Single(x => x.GetAttributeValue("class", null) == "yuan_con");
			Title = info.Descendants("p").Single(x => x.GetAttributeValue("class", null) == "hh").InnerText;
			Description = info.Descendants("p").Single(x => x.GetAttributeValue("class", null) == "ie2").InnerText;
			Tags = info.Descendants("span").Select(x => x.InnerText).ToList();

			var authorInfo = div.Single(x => x.GetAttributeValue("class", null) == "yuan_img");
			AuthorIconUrl = new Uri($"https:{authorInfo.Descendants("img").Single().GetAttributeValue("src", null)}");
			AuthorUsername = authorInfo.Descendants("p").Single().InnerText;
			AuthorId = Convert.ToUInt64(node.GetAttributeValue("data-user_id", null).Split('_').Last()).ToString(); //Make sure valid number;
		}

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => await DiyidanPostDownloader.GetDiyidanImagesAsync(client, PostUrl).CAF();
		/// <summary>
		/// Returns the post id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}