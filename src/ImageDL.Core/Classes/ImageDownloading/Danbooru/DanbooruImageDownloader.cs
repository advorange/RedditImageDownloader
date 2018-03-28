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

namespace ImageDL.Classes.ImageDownloading.Danbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	public sealed class DanbooruImageDownloader : ImageDownloader<DanbooruPost>
	{
		/// <summary>
		/// The terms to search for. Split by spaces. Max allowed is two.
		/// </summary>
		public string TagString
		{
			get => _TagString;
			set
			{
				if (value.Split(' ').Length > 2)
				{
					throw new ArgumentException("Cannot search for more than two tags.", nameof(TagString));
				}
				_TagString = value;
				NotifyPropertyChanged();
			}
		}
		/// <summary>
		/// The page to start downloading from.
		/// </summary>
		public int Page
		{
			get => _Page;
			set
			{
				_Page = Math.Min(1, value);
				NotifyPropertyChanged();
			}
		}

		private string _TagString;
		private int _Page;

		/// <summary>
		/// Creates an image downloader for Danbooru.
		/// </summary>
		public DanbooruImageDownloader()
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
		}

		/// <inheritdoc />
		protected override async Task<List<DanbooruPost>> GatherPostsAsync()
		{
			var validPosts = new List<DanbooruPost>();
			try
			{
				//Uses for instead of while to save 2 lines.
				for (int i = 0; validPosts.Count < AmountToDownload; ++i)
				{
					//Limit caps out at 200 per pages, so can't get this all in one iteration. Have to keep incrementing page.
					var search = $"https://danbooru.donmai.us/posts.json" +
						$"?utf8=✓" +
						$"&tags={WebUtility.UrlEncode(TagString)}" +
						$"&limit={AmountToDownload}" +
						$"&page={Page + i}";

					var json = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(search), TimeSpan.FromSeconds(2)).CAF();

					//Deserialize the Json and look through all the posts
					var finished = false;
					var posts = JsonConvert.DeserializeObject<List<DanbooruPost>>(json);
					foreach (var post in posts)
					{
						if (post.CreatedAt.ToUniversalTime() < OldestAllowed)
						{
							finished = true;
							break;
						}
						else if (!FitsSizeRequirements(null, post.ImageWidth, post.ImageHeight, out _) || post.Score < MinScore)
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
					if (finished || posts.Count < Math.Min(AmountToDownload, 200))
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
				Console.WriteLine($"Finished gathering Danbooru posts.");
				Console.WriteLine();
			}
			return validPosts.OrderByDescending(x => x.Score).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(DanbooruPost post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Score}] http://danbooru.donmai.us/posts/{post.Id}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(DanbooruPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{post.TagStringArtist}_{post.TagStringCharacter}".Replace(' ', '_');
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(DanbooruPost post)
		{
			if (!Uri.TryCreate(post.FileUrl, UriKind.Absolute, out var uri))
			{
				uri = new Uri($"https://danbooru.donmai.us{post.FileUrl}");
			}
			return await Client.ScrapeImagesAsync(uri).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DanbooruPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
	}
}