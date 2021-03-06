﻿using System;
using System.Linq;
using System.Threading.Tasks;

using AdvorangesUtils;

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
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => Post.CreatedUTC;

		/// <summary>
		/// The post holding all of the information.
		/// </summary>
		[JsonProperty("post")]
		public Post Post { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.reddit.com/{Id}");

		/// <inheritdoc />
		[JsonIgnore]
		public string Id => Post.Id;

		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Post.Score;

		/// <summary>
		/// Creates an instance of <see cref="RedditPost"/>.
		/// </summary>
		/// <param name="post"></param>
		public RedditPost(Post post)
		{
			Post = post;
		}

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			if (Post.Url.ToString().IsImagePath())
			{
				return ImageResponse.FromUrl(Post.Url);
			}
			if (client.Gatherers.SingleOrDefault(x => x.IsFromWebsite(Post.Url)) is IImageGatherer gatherer)
			{
				return await gatherer.FindImagesAsync(client, Post.Url).CAF();
			}
			return ImageResponse.FromNotFound(PostUrl);
		}

		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=> Id;
	}
}