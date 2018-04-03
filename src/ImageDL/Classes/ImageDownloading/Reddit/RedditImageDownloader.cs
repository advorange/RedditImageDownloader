using AdvorangesUtils;
using ImageDL.Classes.SettingParsing;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Reddit.RedditPost;

namespace ImageDL.Classes.ImageDownloading.Reddit
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public sealed class RedditImageDownloader : ImageDownloader<Model>
	{
		/// <summary>
		/// The subreddit to download images from.
		/// </summary>
		public string Subreddit
		{
			get => _Subreddit;
			set => _Subreddit = value;
		}

		private RedditSharp.Reddit _Reddit;
		private string _Subreddit;

		/// <summary>
		/// Creates an image downloader for reddit.
		/// </summary>
		/// <param name="client">The client to download images with.</param>
		public RedditImageDownloader(ImageDownloaderClient client) : base(client, new Uri("https://www.reddit.com"))
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Subreddit), "sr" }, x => Subreddit = x)
			{
				Description = "The subreddit to download images from.",
			});

			_Reddit = new RedditSharp.Reddit(new WebAgent(), false);
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<Model> list)
		{
			var valid = new CancellationTokenSource();
			var subreddit = await _Reddit.GetSubredditAsync(Subreddit).CAF();
			try
			{
				await subreddit.GetPosts(RedditSharp.Things.Subreddit.Sort.New, int.MaxValue).ForEachAsync(post =>
				{
					if (post.CreatedUTC < OldestAllowed)
					{
						valid.Cancel();
						valid.Token.ThrowIfCancellationRequested();
					}
					else if (post.IsStickied || post.IsSelfPost || post.Score < MinScore)
					{
						return;
					}
					else if (!Add(list, new Model(post)))
					{
						valid.Cancel();
						valid.Token.ThrowIfCancellationRequested();
					}
				}, valid.Token).CAF();
			}
			catch (OperationCanceledException) { }
		}
	}
}