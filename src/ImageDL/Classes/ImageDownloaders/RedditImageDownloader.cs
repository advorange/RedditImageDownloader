using ImageDL.Classes.ImageGatherers;
using ImageDL.Utilities;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloaders
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public sealed class RedditImageDownloader : GenericImageDownloader<Post>
	{
		/// <summary>
		/// The subreddit to download images from.
		/// </summary>
		public string Subreddit
		{
			get => _Subreddit;
			set
			{
				_Subreddit = value;
				NotifyPropertyChanged(_Subreddit);
			}
		}
		/// <summary>
		/// The minimum score a thread can have before images won't be downloaded from it.
		/// </summary>
		public int MinScore
		{
			get => _MinScore;
			set
			{
				_MinScore = Math.Max(0, value);
				NotifyPropertyChanged(_MinScore);
			}
		}

		private Reddit _Reddit = new Reddit(new WebAgent(), false);
		private string _Subreddit;
		private int _MinScore;

		public RedditImageDownloader() : base()
		{
			CommandLineParserOptions.Add($"sr|subreddit|{nameof(Subreddit)}=", "the subreddit to download images from.", i => SetValue<string>(i, c => Subreddit = c));
			CommandLineParserOptions.Add($"ms|mins|{nameof(MinScore)}=", "the minimum score for an image to have before being ignored.", i => SetValue<int>(i, c => MinScore = c));
		}

		protected override async Task<List<Post>> GatherPostsAsync()
		{
			var validPosts = new List<Post>();
			try
			{
				var oldestAllowed = DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
				var subreddit = await _Reddit.GetSubredditAsync(Subreddit).ConfigureAwait(false);
				var valid = new CancellationTokenSource();
				await subreddit.GetPosts(RedditSharp.Things.Subreddit.Sort.New, AmountToDownload).ForEachAsync(post =>
				{
					if (post.CreatedUTC < oldestAllowed)
					{
						valid.Cancel();
						valid.Token.ThrowIfCancellationRequested();
					}
					else if (post.IsStickied || post.IsSelfPost || post.Score < MinScore)
					{
						return;
					}

					validPosts.Add(post);
					if (validPosts.Count == AmountToDownload)
					{
						valid.Cancel();
						valid.Token.ThrowIfCancellationRequested();
					}
					else if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} reddit posts found.");
					}
				}, valid.Token).ConfigureAwait(false);
				Console.WriteLine($"Finished gathering reddit posts.");
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"Finished gathering reddit posts.");
			}
			catch (Exception e)
			{
				e.Write();
			}
			Console.WriteLine();
			return validPosts.OrderByDescending(x => x.Score).ToList();
		}
		protected override void WritePostToConsole(Post post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.Url}");
		}
		protected override string GenerateFileName(Post post, WebResponse response, Uri uri)
		{
			var gottenName = response.Headers["Content-Disposition"] ?? response.ResponseUri.LocalPath ?? uri.ToString();
			var totalName = $"{post.Id}_{gottenName.Substring(gottenName.LastIndexOf('/') + 1)}";
			return new string(totalName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
		}
		protected override async Task<ImageGatherer> CreateGathererAsync(Post post)
		{
			return await ImageGatherer.CreateGatherer(post.Url).ConfigureAwait(false);
		}
		protected override ContentLink CreateContentLink(Post post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}
