namespace ImageDL.Classes.ImageDownloading.Reddit
{
	/// <summary>
	/// Holds the gotten post for reddit.
	/// </summary>
	public sealed class RedditPost : Post
	{
		/// <summary>
		/// The post holding all of the information.
		/// </summary>
		public readonly RedditSharp.Things.Post Post;

		/// <inheritdoc />
		public override string Link => $"https://www.reddit.com/{Id}";
		/// <inheritdoc />
		public override string Id => Post.Id;
		/// <inheritdoc />
		public override int Score => Post.Score;

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
