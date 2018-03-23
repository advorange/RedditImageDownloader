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
	public sealed class RedditImageDownloader : ImageDownloader<Post>
	{
		/// <summary>
		/// The subreddit to download images from.
		/// </summary>
		public string Subreddit
		{
			get => _Subreddit;
			set => NotifyPropertyChanged(_Subreddit = value);
		}
		/// <summary>
		/// The minimum score a thread can have before images won't be downloaded from it.
		/// </summary>
		public int MinScore
		{
			get => _MinScore;
			set => NotifyPropertyChanged(_MinScore = Math.Max(0, value));
		}

		private Reddit _Reddit;
		private string _Subreddit;
		private int _MinScore;

		public RedditImageDownloader()
		{
			CommandLineParserOptions.Add($"sr|subreddit|{nameof(Subreddit)}=", "the subreddit to download images from.", i => SetValue<string>(i, c => Subreddit = c));
			CommandLineParserOptions.Add($"ms|mins|{nameof(MinScore)}=", "the minimum score for an image to have before being ignored.", i => SetValue<int>(i, c => MinScore = c));

			_Reddit = new Reddit(new WebAgent(), false);
		}

		/// <inheritdoc />
		protected override async Task<List<Post>> GatherPostsAsync()
		{
			var validPosts = new List<Post>();
			try
			{
				var valid = new CancellationTokenSource();
				var subreddit = await _Reddit.GetSubredditAsync(Subreddit).ConfigureAwait(false);
				await subreddit.GetPosts(RedditSharp.Things.Subreddit.Sort.New, AmountToDownload).ForEachAsync(post =>
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
			}
			catch (OperationCanceledException) { }
			catch (Exception e)
			{
				e.Write();
			}
			finally
			{
				Console.WriteLine($"Finished gathering reddit posts.");
				Console.WriteLine();
			}
			return validPosts.OrderByDescending(x => x.Score).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(Post post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.Url}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(Post post, WebResponse response, Uri uri)
		{
			var gottenName = response.Headers["Content-Disposition"] ?? response.ResponseUri.LocalPath ?? uri.ToString();
			var totalName = $"{post.Id}_{gottenName.Substring(gottenName.LastIndexOf('/') + 1)}";
			var validName = new string(totalName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
			return new FileInfo(Path.Combine(Directory, validName));
		}
		/// <inheritdoc />
		protected override async Task<ImageGatherer> CreateGathererAsync(Post post)
		{
			return await ImageGatherer.CreateGathererAsync(Scrapers, post.Url).ConfigureAwait(false);
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(Post post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}
