using System;
using System.Collections.Generic;
using ImageDL.Interfaces;

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
		public readonly RedditSharp.Things.Post Post;

		/// <inheritdoc />
		public string Id => Post.Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri($"https://www.reddit.com/{Id}");
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => new[] { Post.Url };
		/// <inheritdoc />
		public int Score => Post.Score;
		/// <inheritdoc />
		public DateTime CreatedAt => Post.CreatedUTC;

		/// <summary>
		/// Creates an instance of <see cref="RedditPost"/>.
		/// </summary>
		/// <param name="post"></param>
		public RedditPost(RedditSharp.Things.Post post)
		{
			Post = post;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Id;
		}
	}
}