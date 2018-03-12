using ImageDL.Classes.ImageGatherers;
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
	public sealed class DanbooruImageDownloader : GenericImageDownloader<DanbooruPost>
	{
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
		public int Page
		{
			get => _Page;
			set
			{
				_Page = value;
				NotifyPropertyChanged(_Page);
			}
		}

		private BooruLoader _Booru;
		private DanbooruLoader _Danbooru;
		private string _TagString;
		private int _Page;

		public DanbooruImageDownloader() : base()
		{
			CommandLineParserOptions.Add($"tags|{nameof(TagString)}=", "the tags to search for.", i => SetValue<string>(i, c => TagString = c));
			CommandLineParserOptions.Add($"{nameof(Page)}=", "the page to start from.", i => SetValue<int>(i, c => Page = c));

			_Booru = new BooruLoader(null, 1240);
			_Danbooru = new DanbooruLoader(null, null, 1240, null, _Booru);
		}

		protected override async Task<List<DanbooruPost>> GatherPostsAsync()
		{
			var validPosts = new List<DanbooruPost>();
			try
			{
				var oldestAllowed = DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
#if false
				//The method to specify how many to download isn't public for some reason.
				var method = typeof(DanbooruLoader).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
					.Single(x => x.Name == nameof(DanbooruLoader.LoadSearchResultAsync));
				var invoked = method.Invoke(_Danbooru, new object[] { _TagString, (int?)AmountToDownload });
				var searchResults = await ((Task<SearchResult>)invoked).ConfigureAwait(false);
#endif
				var search = $"https://danbooru.donmai.us/posts.json?utf8=✓&tags={WebUtility.UrlEncode(TagString)}&limit={AmountToDownload}&page={Page}";
				var pageHtml = await _Booru.LoadPageAsync(search).ConfigureAwait(false);
				var searchResults = new DanbooruSearchResult(JsonConvert.DeserializeObject<List<Model>>(pageHtml));
				foreach (var searchResult in searchResults.Results)
				{
					var post = (DanbooruPost)(await _Danbooru.LoadPostAsync(searchResult.Id).ConfigureAwait(false));
					if (post.PostedDateTime.ToUniversalTime() < oldestAllowed)
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
		protected override void WritePostToConsole(DanbooruPost post, int count)
		{
			Console.WriteLine($"[#{count}] {GetPostUri(post)}");
		}
		protected override string GenerateFileName(DanbooruPost post, WebResponse response, Uri uri)
		{
			var gottenName = response.Headers["Content-Disposition"] ?? response.ResponseUri.LocalPath ?? uri.ToString();
			var totalName = $"{post.PostId}_{gottenName.Substring(gottenName.LastIndexOf('/') + 1)}";
			return new string(totalName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
		}
		protected override async Task<ImageGatherer> CreateGathererAsync(DanbooruPost post)
		{
			return await ImageGatherer.CreateGathererAsync(Scrapers, new Uri(GetPostUri(post))).ConfigureAwait(false);
		}
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
