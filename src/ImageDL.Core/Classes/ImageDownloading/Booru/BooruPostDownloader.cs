using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesSettingParser.Implementation.Instance;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Booru.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Booru
{
	/// <summary>
	/// Downloads images from a Booru website.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DownloaderName("Booru")]
	public abstract class BooruPostDownloader<T> : PostDownloader where T : BooruPost
	{
		private readonly bool _Json;

		private readonly int _TagLimit;

		private int _Page = 1;

		private string _Tags;

		/// <summary>
		/// The page to start downloading from.
		/// </summary>
		public int Page
		{
			get => _Page;
			set => _Page = Math.Min(1, value);
		}

		/// <summary>
		/// The terms to search for. Split by spaces.
		/// </summary>
		public string Tags
		{
			get => _Tags;
			set
			{
				if (value.Split(' ').Length > _TagLimit)
				{
					throw new ArgumentException($"Cannot search for more than {_TagLimit} tags.", nameof(Tags));
				}
				_Tags = value;
			}
		}

		//Start on the first page, not 0 indexed.
		/// <summary>
		/// Creats an instance of <see cref="BooruPostDownloader{T}"/>.
		/// </summary>
		/// <param name="tagLimit">The maximum amount of tags allowed to search at a time.</param>
		/// <param name="json">If true, tells the downloader it should be expecting to parse Json. Otherwise parses XML.</param>
		protected BooruPostDownloader(int tagLimit, bool json = true)
		{
			SettingParser.Add(new Setting<string>(() => Tags)
			{
				Description = "The tags to search for.",
			});
			SettingParser.Add(new Setting<int>(() => Page)
			{
				Description = "The page to start from.",
				IsOptional = true,
			});

			_TagLimit = tagLimit;
			_Json = json;
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<T>();
			//Iterate because there's a limit of around 100 per request
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 100); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = GenerateQuery(Tags, Page + i);
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = Parse(result.Value);
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post, out _) || post.Score < MinScore)
					{
						continue;
					}
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// Generates the search query to get images from.
		/// Keep the limit to 100 otherwise some logic may not work in <see cref="GatherAsync(IDownloaderClient, List{IPost}, CancellationToken)"/>.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		protected abstract Uri GenerateQuery(string tags, int page);

		/// <summary>
		/// Converts the text into a list of posts.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected abstract List<T> Parse(string text);
	}
}