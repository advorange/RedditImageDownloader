using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Booru
{
	/// <summary>
	/// Downloads images from a -booru based website.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BooruImageDownloader<T> : ImageDownloader<T> where T : BooruPost
	{
		/// <summary>
		/// The terms to search for. Split by spaces. Max allowed is two.
		/// </summary>
		public string TagString
		{
			get => _TagString;
			set
			{
				if (value.Split(' ').Length > _TagLimit)
				{
					throw new ArgumentException($"Cannot search for more than {_TagLimit} tags.", nameof(TagString));
				}
				_TagString = value;
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

		private string _TagString;
		private int _Page;
		private string _BaseUri;
		private string _Search;
		private int _TagLimit;

		/// <summary>
		/// Creats an instance of <see cref="BooruImageDownloader{T}"/>.
		/// </summary>
		/// <param name="baseUri">The website to search.</param>
		/// <param name="search">For some websites to search for a post it's 'posts.json' others it's 'post.json'</param>
		/// <param name="tagLimit">The maximum amount of tags allowed to search at a time.</param>
		public BooruImageDownloader(Uri baseUri, string search, int tagLimit)
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(TagString), "tags" }, x => TagString = x)
			{
				Description = "The tags to search for.",
			});
			SettingParser.Add(new Setting<int>(new[] { nameof(Page), }, x => Page = x)
			{
				Description = "The page to start from.",
				DefaultValue = 1, //Start on the first page
			});

			_BaseUri = baseUri.ToString().TrimEnd('/');
			_Search = search.Trim('/');
			_TagLimit = tagLimit;
		}

		/// <summary>
		/// Generates the search query to get images from.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		protected virtual string GenerateQuery(int page)
		{
			//Limit caps out at 100 per pages, so can't get this all in one iteration. Have to keep incrementing page.
			return $"{_BaseUri}/{_Search}" +
				$"?utf8=✓" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(TagString)}" +
				$"&page={page}";
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(T post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Score}] http://danbooru.donmai.us/posts/{post.Id}");
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
					var search = GenerateQuery(Page + i);
					var json = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(search), TimeSpan.FromSeconds(2)).CAF();

					//Deserialize the Json and look through all the posts
					var finished = false;
					var posts = JsonConvert.DeserializeObject<List<T>>(json);
					foreach (var post in posts)
					{
						if (post.CreatedAt.ToUniversalTime() < OldestAllowed)
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
					if (finished || posts.Count < Math.Min(AmountToDownload, 100))
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
				Console.WriteLine($"Finished gathering {typeof(T).Name.FormatTitle()[0]} posts.");
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
			if (!Uri.TryCreate(post.FileUrl, UriKind.Absolute, out var uri))
			{
				uri = new Uri($"{_BaseUri}{post.FileUrl}");
			}
			return await Client.ScrapeImagesAsync(uri).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(T post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}
