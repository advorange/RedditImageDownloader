using ImageDL.Classes;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public sealed class RedditImageDownloader : ImageDownloader<Post>
	{
		private Reddit _Reddit = new Reddit(new WebAgent(), false);

		private string _Subreddit;
		[Setting("The subreddit to download images from.")]
		public string Subreddit
		{
			get => _Subreddit;
			set
			{
				_Subreddit = value;
				NotifyArgumentSet();
				NotifyPropertyChanged();
			}
		}
		private int _ScoreThreshold;
		[Setting("The minimum score allowed for an image to be downloaded.")]
		public int ScoreThreshold
		{
			get => _ScoreThreshold;
			set
			{
				_ScoreThreshold = value;
				NotifyArgumentSet();
				NotifyPropertyChanged();
			}
		}

		public RedditImageDownloader() : base() { }
		public RedditImageDownloader(params string[] args) : base(args) { }

		protected override async Task<IEnumerable<Post>> GatherPostsAsync()
		{
			var validPosts = new List<Post>();
			try
			{
				var oldestAllowed = DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));

				var subreddit = await _Reddit.GetSubredditAsync(Subreddit).ConfigureAwait(false);
				foreach (var post in subreddit.New)
				{
					if (post.CreatedUTC < oldestAllowed)
					{
						break;
					}

					if (post.IsStickied || post.IsSelfPost || post.Score < ScoreThreshold)
					{
						continue;
					}

					validPosts.Add(post);
					if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} reddit posts found.");
					}
				}
			}
			catch (WebException e)
			{
				Console.WriteLine(e.Message);
			}
			Console.WriteLine();
			return validPosts;
		}
		protected override void WritePostToConsole(Post post, int count)
			=> Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.Url}");
		protected override async Task<UriImageGatherer> CreateGathererAsync(Post post)
			=> await UriImageGatherer.CreateGatherer(post.Url).ConfigureAwait(false);
		protected override ContentLink CreateContentLink(Post post, Uri uri)
			=> new ContentLink(uri, post.Score);
	}
}
