using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using RedditSharp.Things;

namespace ImageDL.Classes.ImageDownloading.Reddit.Models
{
	/// <summary>
	/// Holds the gotten post for reddit.
	/// </summary>
	public sealed class RedditPost : IPost
	{
		/// <summary>
		/// The post holding all of the information.
		/// </summary>
		[JsonProperty("post")]
		public Post Post { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => Post.Id;
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.reddit.com/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public IEnumerable<Uri> ContentUrls => new[] { Post.Url };
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Post.Score;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => Post.CreatedUTC;

		/// <summary>
		/// Creates an instance of <see cref="RedditPost"/>.
		/// </summary>
		/// <param name="post"></param>
		public RedditPost(Post post)
		{
			Post = post;
		}

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			var url = Post.Url;
			if (url.ToString().IsImagePath())
			{
				return new ImageResponse(FailureReason.Success, null, url);
			}
			else if (client.Gatherers.SingleOrDefault(x => x.IsFromWebsite(url)) is IImageGatherer gatherer)
			{
				return await gatherer.FindImagesAsync(client, url).CAF();
			}
			return new ImageResponse(FailureReason.Misc, null, url);
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