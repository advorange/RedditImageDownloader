using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ImageDL.Classes.ImageDownloading.Booru
{
	/// <summary>
	/// Downloads images from a -booru based website.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BooruImageDownloader<T> : ImageDownloader<T> where T : BooruPost
	{
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
		/// <summary>
		/// The page to start downloading from.
		/// </summary>
		public int Page
		{
			get => _Page;
			set => _Page = Math.Min(1, value);
		}

		private string _Tags;
		private int _Page;
		private int _TagLimit;
		private bool _Json;

		/// <summary>
		/// Creats an instance of <see cref="BooruImageDownloader{T}"/>.
		/// </summary>
		/// <param name="tagLimit">The maximum amount of tags allowed to search at a time.</param>
		/// <param name="json">If true, tells the downloader it should be expecting to parse Json. Otherwise parses XML.</param>
		public BooruImageDownloader(int tagLimit, bool json = true)
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for.",
			});
			SettingParser.Add(new Setting<int>(new[] { nameof(Page), }, x => Page = x)
			{
				Description = "The page to start from.",
				DefaultValue = 1, //Start on the first page
			});

			_TagLimit = tagLimit;
			_Json = json;
		}

		/// <summary>
		/// Generates the search query to get images from.
		/// Keep the limit to 100 otherwise some logic may not work in <see cref="GatherPostsAsync"/>.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		protected abstract string GenerateQuery(int page);
		/// <summary>
		/// Converts the text into a list of posts.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		protected abstract List<T> Parse(string text);
		/// <inheritdoc />
		protected override void WritePostToConsole(T post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.PostUrl}");
		}
		/// <inheritdoc />
		protected override async Task<List<T>> GatherPostsAsync()
		{
			var validPosts = new List<T>();
			try
			{
				//Uses for instead of while to save 2 lines.
				for (int i = 0; validPosts.Count < AmountToDownload; ++i)
				{
					var finished = false;
					//Limit caps out at 100 per pages, so can't get this all in one iteration. Have to keep incrementing page.
					var text = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(GenerateQuery(Page + i)), TimeSpan.FromSeconds(2)).CAF();
					//Deserialize the text and look through all the posts
					var posts = Parse(text);
					foreach (var post in posts)
					{
						if (post.CreatedAt < OldestAllowed)
						{
							finished = true;
							break;
						}
						else if (!FitsSizeRequirements(null, post.Width, post.Height, out _) || post.Score < MinScore)
						{
							continue;
						}

						validPosts.Add(post);
						if (validPosts.Count == AmountToDownload)
						{
							finished = true;
							break;
						}
						else if (validPosts.Count % 25 == 0)
						{
							Console.WriteLine($"{validPosts.Count} Danbooru posts found.");
						}
					}

					//Anything less than a full page means everything's been searched
					if (finished || posts.Count < 100)
					{
						break;
					}
				}
			}
			catch (Exception e)
			{
				e.Write();
			}
			finally
			{
				Console.WriteLine($"Finished gathering {typeof(T).Name.FormatTitle().Split(' ')[0]} posts.");
				Console.WriteLine();
			}
			return validPosts.OrderByDescending(x => x.Score).ToList();
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(T post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{post.FileUrl.Split('/').Last()}".Replace(' ', '_');
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(T post)
		{
			return await Client.ScrapeImagesAsync(new Uri(post.FileUrl)).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(T post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}
