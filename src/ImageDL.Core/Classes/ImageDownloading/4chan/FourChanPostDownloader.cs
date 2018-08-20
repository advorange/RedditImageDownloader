using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using FChan.Library;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.FourChan.Models.FourChanPost;

namespace ImageDL.Classes.ImageDownloading.FourChan
{
	/// <summary>
	/// Downloads images from 4chan.
	/// </summary>
	[DownloaderName("4chan")]
	public sealed class FourChanPostDownloader : PostDownloader
	{
		/// <summary>
		/// The board to download images from.
		/// </summary>
		public string Board { get; set; }
		/// <summary>
		/// The thread to download images from.
		/// </summary>
		public int ThreadId { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="FourChanPostDownloader"/>.
		/// </summary>
		public FourChanPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Board), }, x => Board = x)
			{
				Description = "The board to download images from.",
			});
			SettingParser.Add(new Setting<int>(new[] { nameof(ThreadId), }, x => ThreadId = x)
			{
				Description = "The id of the thread to download images from. If left default, will download all threads.",
				IsOptional = true,
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			if (ThreadId > 0)
			{
				ProcessThread(list, await Chan.GetThreadAsync(Board, ThreadId).CAF(), ThreadId, token);
				return;
			}
			for (int i = 1; i < 10; ++i)
			{
				foreach (var thread in (await Chan.GetThreadPageAsync(Board, i).CAF()).Threads)
				{
					if (!ProcessThread(list, thread, thread.Posts[0].PostNumber, token))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// Grabs all the images from the thread.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="thread"></param>
		/// <param name="threadId"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool ProcessThread(List<IPost> list, FChan.Library.Thread thread, int threadId, CancellationToken token)
		{
			for (int i = 0; i < thread.Posts.Count && list.Count < AmountOfPostsToGather; ++i)
			{
				token.ThrowIfCancellationRequested();
				var post = new Model(thread.Posts[i], threadId);
				//Return true because we want to stop processing this thread, but the other threads may still be processable
				if (post.CreatedAt < OldestAllowed)
				{
					return true;
				}
				if (!HasValidSize(post, out _) || post.Post.IsStickied || post.Post.IsArchived || (post.Post.IsFileDeleted ?? false))
				{
					continue;
				}
				if (!Add(list, post))
				{
					return false;
				}
			}
			return list.Count < AmountOfPostsToGather;
		}
	}
}
