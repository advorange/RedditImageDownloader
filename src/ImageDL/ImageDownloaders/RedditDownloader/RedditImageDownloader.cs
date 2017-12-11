using ImageDL.Classes;
using ImageDL.Enums;
using ImageDL.HelperClasses;
using RedditSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageDL.ImageDownloaders.RedditDownloader
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public class RedditImageDownloader : ImageDownloader<RedditImageDownloaderArguments>
	{
		private Reddit _Reddit = new Reddit(new WebAgent(), false);

		/// <summary>
		/// Download images with the supplied arguments.
		/// </summary>
		/// <param name="args">The supplied information about what to download.</param>
		protected override void DownloadImages(RedditImageDownloaderArguments args)
		{
			var subreddit = this._Reddit.GetSubreddit(args.Subreddit);
			var validPosts = subreddit.Hot.Where(x => !x.IsStickied && !x.IsSelfPost && x.Score >= args.ScoreThreshold);
			var posts = validPosts.Take(args.AmountToDownload);

			//Look through each post
			var element = 0;
			foreach (var post in posts)
			{
				Thread.Sleep(25);
				Console.WriteLine($"[#{++element}|\u2191{post.Score}] {post.Url}");
				//Some links might have more than one image
				foreach (var uri in UriHelper.GetImageUris(post.Url))
				{
					switch (UriHelper.CorrectUri(uri, out var correctedUri))
					{
						case UriCorrectionResponse.Valid:
						case UriCorrectionResponse.Unknown:
						{
							var fileName = $"{post.Shortlink.Split('/').Last()}_{correctedUri.ToString().Split('/').Last()}";
							DownloadImage(correctedUri, new DirectoryInfo(args.Folder), fileName);
							continue;
						}
						case UriCorrectionResponse.Animated:
						{
							this._AnimatedContent.Add(new AnimatedContent(correctedUri, post.Score));
							continue;
						}
						case UriCorrectionResponse.Invalid:
						{
							continue;
						}
					}
				}
			}
		}
	}
}
