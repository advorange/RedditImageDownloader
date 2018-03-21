using ImageDL.Classes.ImageGatherers;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using Imouto.BooruParser.Loaders;
using Imouto.BooruParser.Model.Danbooru;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Imouto.BooruParser.Model.Danbooru.Json.Post;

namespace ImageDL.Classes.ImageDownloaders
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
				NotifyPropertyChanged(_TagString);
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
				NotifyPropertyChanged(_Page);
			}
		}

		private BooruLoader _Booru;
		private DanbooruLoader _Danbooru;
		private string _TagString;
		private int _Page;

		public DanbooruImageDownloader(IImageComparer imageComparer) : base(imageComparer)
		{
			CommandLineParserOptions.Add($"tags|{nameof(TagString)}=", "the tags to search for.", i => SetValue<string>(i, c => TagString = c));
			CommandLineParserOptions.Add($"{nameof(Page)}=", "the page to start from.", i => SetValue<int>(i, c => Page = c));

			_Booru = new BooruLoader(null, 1240);
			_Danbooru = new DanbooruLoader(null, null, 1240, null, _Booru);

			Page = 1;
		}

		/// <inheritdoc />
		protected override async Task<List<DanbooruPost>> GatherPostsAsync()
		{
			var validPosts = new List<DanbooruPost>();
			try
			{
				var search = $"https://danbooru.donmai.us/posts.json?utf8=✓" +
					$"&tags={WebUtility.UrlEncode(TagString)}" +
					$"&limit={AmountToDownload}" +
					$"&page={Page}";
				var pageHtml = await _Booru.LoadPageAsync(search).ConfigureAwait(false);
				var searchResults = new DanbooruSearchResult(JsonConvert.DeserializeObject<List<Model>>(pageHtml));
				foreach (var searchResult in searchResults.Results)
				{
					var post = (DanbooruPost)(await _Danbooru.LoadPostAsync(searchResult.Id).ConfigureAwait(false));
					if (post.PostedDateTime.ToUniversalTime() < OldestAllowed)
					{
						break;
					}
					else if (post.ImageSize.Width < MinWidth || post.ImageSize.Height < MinHeight)
					{
						Console.WriteLine($"{GetPostUri(post)} is too small ({post.ImageSize.Width}x{post.ImageSize.Height}).");
					}

					validPosts.Add(post);
				}
				Console.WriteLine("Finished gathering danbooru posts.");
			}
			catch (Exception e)
			{
				e.Write();
			}
			Console.WriteLine();
			return validPosts.OrderBy(x => x.PostedDateTime).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(DanbooruPost post, int count)
		{
			Console.WriteLine($"[#{count}] {GetPostUri(post)}");
		}
		/// <inheritdoc />
		protected override string GenerateFileName(DanbooruPost post, WebResponse response, Uri uri)
		{
			var gottenName = response.Headers["Content-Disposition"] ?? response.ResponseUri.LocalPath ?? uri.ToString();
			var totalName = $"{post.PostId}_{gottenName.Substring(gottenName.LastIndexOf('/') + 1)}";
			return new string(totalName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
		}
		/// <inheritdoc />
		protected override async Task<ImageGatherer> CreateGathererAsync(DanbooruPost post)
		{
			return await ImageGatherer.CreateGathererAsync(Scrapers, new Uri(GetPostUri(post))).ConfigureAwait(false);
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DanbooruPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.PostId, reason);
		}

		private string GetPostUri(DanbooruPost post)
		{
			return $"http://danbooru.donmai.us/posts/{post.PostId}";
		}
	}
}
