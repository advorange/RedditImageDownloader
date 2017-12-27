using ImageDL.Classes;
using ImageDL.Utilities;
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
				AddArgumentToSetArguments();
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
				AddArgumentToSetArguments();
			}
		}

		public RedditImageDownloader(params string[] args) : base(args) { }

		protected override async Task<IEnumerable<Post>> GatherPostsAsync()
		{
			try
			{
				var subreddit = await _Reddit.GetSubredditAsync(Subreddit);
				var validPosts = subreddit.Hot.Where(x =>
				{
					//Don't allow if it's not going to be an image
					if (x.IsStickied || x.IsSelfPost)
					{
						return false;
					}
					//Don't allow if scored too low
					else if (x.Score < ScoreThreshold)
					{
						return false;
					}
					//Don't allow if too old
					else if (x.CreatedUTC < DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld)))
					{
						return false;
					}
					return true;
				});
				return validPosts.Take(AmountToDownload);
			}
			catch (WebException e)
			{
				Console.WriteLine(e.Message);
			}
			return Enumerable.Empty<Post>();
		}
		protected override IEnumerable<Uri> GatherImages(Post post)
			=> UriUtils.GetImageUris(post.Url);
		protected override void WritePostToConsole(Post post, int count)
			=> Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.Url}");
		protected override ContentLink CreateContentLink(Post post, Uri uri)
			=> new ContentLink(uri, post.Score);
	}
}
