using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Reddit
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

		private RedditSharp.Reddit _Reddit;
		private string _Subreddit;

		/// <summary>
		/// Creates an image downloader for reddit.
		/// </summary>
		public RedditImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Subreddit), "sr" }, x => Subreddit = x)
			{
				Description = "The subreddit to download images from.",
				IsFlag = true,
			});

			_Reddit = new RedditSharp.Reddit(new WebAgent(), false);
		}

		/// <inheritdoc />
		protected override async Task<List<Post>> GatherPostsAsync()
		{
			var validPosts = new List<Post>();
			try
			{
				var valid = new CancellationTokenSource();
				var subreddit = await _Reddit.GetSubredditAsync(Subreddit).CAF();
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
				}, valid.Token).CAF();
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
		protected override FileInfo GenerateFileInfo(Post post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(Post post)
		{
			return await Client.ScrapeImagesAsync(post.Url).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(Post post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}
