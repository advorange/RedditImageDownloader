﻿using ImageDL.Classes;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public sealed class RedditImageDownloader : GenericImageDownloader<Post>
	{
		private Reddit _Reddit = new Reddit(new WebAgent(), false);

		private string _Subreddit;
		public string Subreddit
		{
			get => _Subreddit;
			set
			{
				_Subreddit = value;
				NotifyPropertyChanged();
			}
		}
		private int _ScoreThreshold;
		public int ScoreThreshold
		{
			get => _ScoreThreshold;
			set
			{
				_ScoreThreshold = value;
				NotifyPropertyChanged();
			}
		}

		public RedditImageDownloader() : base()
		{
			CommandLineParserOptions.Add("sr|subreddit=", "the subreddit to download images from.", x => Subreddit = x);
			CommandLineParserOptions.Add("st|score=", "the minimum score for an image to have before being ignored.", x => ScoreThreshold = Convert.ToInt32(x));
		}

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
					else if (post.IsStickied || post.IsSelfPost || post.Score < ScoreThreshold)
					{
						continue;
					}

					validPosts.Add(post);
					if (validPosts.Count == AmountToDownload)
					{
						Console.WriteLine($"Finished gathering reddit posts.");
						break;
					}
					else if (validPosts.Count % 25 == 0)
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
			return validPosts.OrderByDescending(x => x.Score);
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
		protected override async Task<UriImageGatherer> CreateGathererAsync(Post post)
		{
			return await UriImageGatherer.CreateGatherer(post.Url).ConfigureAwait(false);
		}
		protected override ContentLink CreateContentLink(Post post, Uri uri)
		{
			return new ContentLink(uri, post.Score);
		}
	}
}
