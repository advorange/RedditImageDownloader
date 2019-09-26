using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesSettingParser.Implementation.Instance;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Interfaces;

using RedditSharp;

using Model = ImageDL.Classes.ImageDownloading.Reddit.Models.RedditPost;

namespace ImageDL.Classes.ImageDownloading.Reddit
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	[DownloaderName("Reddit")]
	public sealed class RedditPostDownloader : PostDownloader
	{
		private static RedditSharp.Reddit _Reddit = new RedditSharp.Reddit(new WebAgent(), false);

		/// <summary>
		/// The subreddit to download images from.
		/// </summary>
		public string Subreddit { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="RedditPostDownloader"/>.
		/// </summary>
		public RedditPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Subreddit, new[] { "sr" })
			{
				Description = "The subreddit to download images from.",
			});
		}

		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetRedditImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}

			const string search = "/comments/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetRedditPostAsync(id).CAF() is Model model)
				{
					return await model.GetImagesAsync(client).CAF();
				}
			}

			var parts = url.LocalPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1 && await GetRedditPostAsync(parts[0]).CAF() is Model post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetRedditPostAsync(string id)
		{
			try
			{
				return new Model((RedditSharp.Things.Post)await _Reddit.GetThingByFullnameAsync($"t3_{id}").CAF());
			}
			catch (ArgumentOutOfRangeException) //Returns this when a post was unable to be gotten
			{
				return null;
			}
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			using var valid = new CancellationTokenSource();

			var subreddit = await _Reddit.GetSubredditAsync(Subreddit).CAF();
			try
			{
				await subreddit.GetPosts(RedditSharp.Things.Subreddit.Sort.New, int.MaxValue).ForEachAsync(post =>
				{
					valid.Token.ThrowIfCancellationRequested();
					token.ThrowIfCancellationRequested();
					if (post.CreatedUTC < OldestAllowed)
					{
						valid.Cancel();
					}
					else if (post.IsStickied || post.IsSelfPost || post.Score < MinScore)
					{
						return;
					}
					else if (!Add(list, new Model(post)))
					{
						valid.Cancel();
					}
				}, valid.Token).CAF();
			}
			catch (OperationCanceledException) when (!token.IsCancellationRequested)
			{
			}
		}
	}
}