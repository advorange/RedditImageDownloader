using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageGatherers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloaders
{
	/// <summary>
	/// Downloads images from DeviantArt.
	/// </summary>
	public sealed class DeviantArtImageDownloader : ImageDownloader<DeviantArtPost>
	{
		private const string SEARCH = "https://www.deviantartsupport.com/en/article/are-there-any-tricks-to-narrowing-down-a-search-on-deviantart";
		private static List<string> _Sorts = new List<string>
		{
			"newest",
			"popular-8-hours",
			"popular-24-hours",
			"popular-3-days",
			"popular-1-week",
			"popular-1-month",
			"popular-all-time",
		};
		private static JsonSerializer _Serializer = new JsonSerializer
		{
			MissingMemberHandling = MissingMemberHandling.Error, //Throw error so default instances aren't free to roam
		};

		/// <summary>
		/// The username of the DeviantArt user to search.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => NotifyPropertyChanged(_Username = value);
		}
		/// <summary>
		/// The sort to use for searching.
		/// </summary>
		public string Sort
		{
			get => _Sort;
			set
			{
				if (!_Sorts.CaseInsContains(value))
				{
					throw new ArgumentException($"{nameof(Sort)} must be one of the following: {String.Join(", ", _Sorts)}", nameof(Sort));
				}
				NotifyPropertyChanged(_Sort = value.ToLower());
			}
		}
		/// <summary>
		/// The tags to search for.
		/// </summary>
		public string TagString
		{
			get => _TagString;
			set => NotifyPropertyChanged(_TagString = value);
		}

		private string _Username;
		private string _Sort;
		private string _TagString;

		/// <summary>
		/// Creates an image downloader for DeviantArt.
		/// </summary>
		public DeviantArtImageDownloader()
		{
			CommandLineParserOptions.Add($"user|{nameof(Username)}=", "the user gather images from.", i => SetValue<string>(i, c => Username = c));
			CommandLineParserOptions.Add($"sort|{nameof(Sort)}=", "the sort to use when searching.", i => SetValue<string>(i, c => Sort = c));
			CommandLineParserOptions.Add($"tags|{nameof(TagString)}=", $"the tags to search for. For additional help, visit {SEARCH}", i => SetValue<string>(i, c => TagString = c));

			Sort = _Sorts[0];
		}

		/// <inheritdoc />
		protected override async Task<List<DeviantArtPost>> GatherPostsAsync()
		{
			var validPosts = new List<DeviantArtPost>();
			try
			{
				var keepGoing = true;
				for (int i = 0; keepGoing && validPosts.Count < AmountToDownload;)
				{
					string html;
					using (var resp = await WebsiteScraper.CreateWebRequest(new Uri(GenerateQuery(i))).GetResponseAsync().CAF())
					using (var reader = new StreamReader(resp.GetResponseStream()))
					{
						html = await reader.ReadToEndAsync().CAF();
					}

					var jsonStart = "window.__pageload =";
					var jsonStartIndex = html.IndexOf(jsonStart) + jsonStart.Length;
					var jsonEnd = "}}}</script>";
					var jsonEndIndex = html.IndexOf(jsonEnd) + 3;

					//Now we have all the json, but we only want the artwork json so we have to parse that manually
					var jObj = JObject.Parse(html.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex).Trim());
					var curPostCount = i;
					foreach (var jToken in jObj["metadata"])
					{
						try
						{
							var post = jToken.First.ToObject<DeviantArtPost>(_Serializer);
							++i; //Getting to here means it was successfully deserialized, meaning it's a post

							if (!FitsSizeRequirements(null, post.Width, post.Height, out _)
								|| (MinScore > 0 && await post.GetFavoritesCountAsync().CAF() < MinScore))
							{
								continue;
							}

							validPosts.Add(post);
							if (validPosts.Count == AmountToDownload)
							{
								keepGoing = false;
								break;
							}
							else if (validPosts.Count % 25 == 0)
							{
								Console.WriteLine($"{validPosts.Count} DeviantArt posts found.");
							}
						}
						catch (JsonSerializationException) //Ignore failed deserialization
						{
							continue;
						}
					}

					//Less than 24 (full page) means no more are left.
					if (i - curPostCount < 24)
					{
						break;
					}
				}
			}
			catch (WebException we) when (we.Message.Contains("403")) { } //Eat this error due to not being able to know when to stop
			catch (Exception e)
			{
				e.Write();
			}
			finally
			{
				Console.WriteLine($"Finished gathering DeviantArt posts.");
				Console.WriteLine();
			}
			return validPosts.GroupBy(x => x.Source).Select(x => x.First()).OrderByDescending(x => x.Id).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(DeviantArtPost post, int count)
		{
			Console.WriteLine($"[#{count}] {post.Source}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(DeviantArtPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ImageGatherer> CreateGathererAsync(DeviantArtPost post)
		{
			return await ImageGatherer.CreateGathererAsync(Scrapers, new Uri(post.Source)).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DeviantArtPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Id, reason);
		}

		private string GenerateQuery(int offset)
		{
			var query = WebUtility.UrlEncode(TagString);
			if (MaxDaysOld > 0)
			{
				query += $"+max_age:{MaxDaysOld}d";
			}
			if (!String.IsNullOrWhiteSpace(Username))
			{
				query += $"+by:{WebUtility.UrlEncode(Username)}";
			}

			return $"https://www.deviantart.com/{Sort}/?offset={offset}&q={query}";
		}
	}

	/// <summary>
	/// Json model for a DeviantArt post that is viewed in a gallery (not directly linked).
	/// </summary>
	public class DeviantArtPost
	{
#pragma warning disable 1591 //Disabled since most of these are self explanatory and this is a glorified Json model
		[JsonProperty("mature")]
		public readonly bool Mature;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("sizing")]
		public readonly List<Size> Sizes;
		[JsonProperty("row")]
		public readonly int Row;
		[JsonProperty("faved")]
		public readonly bool Faved;
		[JsonProperty("alt")]
		public readonly string AltText;
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("author")]
		public readonly AuthorInfo Author;
		[JsonProperty("src")]
		public readonly string Source;
		[JsonProperty("type")]
		public readonly string Type;

		/// <summary>
		/// Holds information about a resized version of the image.
		/// </summary>
		public class Size
		{
			[JsonProperty("width")]
			public readonly int Width;
			[JsonProperty("height")]
			public readonly int Height;
			[JsonProperty("src")]
			public readonly string Source;
			[JsonProperty("transparent")]
			public readonly bool Transparent;
		}

		/// <summary>
		/// Holds information about the author of the image.
		/// </summary>
		public class AuthorInfo
		{
			[JsonProperty("symbol")]
			public readonly string Symbol;
			[JsonProperty("username")]
			public readonly string Username;
			[JsonProperty("userid")]
			public readonly int UserId;
			[JsonProperty("usericon")]
			public readonly string UserIcon;
			[JsonProperty("uuid")]
			public readonly string UUID;
			[JsonProperty("attributes")]
			public readonly long Attributes;
		}
#pragma warning restore 1591

		/// <summary>
		/// Gets the amount of favorites the post has.
		/// Will get the exact number for 0 to 100, rounds down otherwise.
		/// Will be slow because it has to load a webpage.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Yes, I know this is retarded.</remarks>
		public async Task<int> GetFavoritesCountAsync()
		{
			if (_FavoritesCount != null)
			{
				return _FavoritesCount.Value;
			}

			WebResponse resp = null;
			Stream s = null;
			try
			{
				var uri = new Uri($"https://www.deviantart.com/deviation/{Id}/favourites");

				var doc = new HtmlDocument();
				doc.Load(s = (resp = await WebsiteScraper.CreateWebRequest(uri).GetResponseAsync().CAF()).GetResponseStream());

				var p = doc.DocumentNode.Descendants("p");
				if (p.Any(x => x.InnerText == "Deviation has no favourites currently."))
				{
					return (_FavoritesCount = 0).Value;
				}

				var div = doc.DocumentNode.Descendants("div");
				if (div.Any(x => x.GetAttributeValue("class", null) == "pagination both-paddles-disabled"))
				{
					var list = doc.DocumentNode.Descendants("ul").Single(x => x.GetAttributeValue("class", null) == "f list");
					return (_FavoritesCount = list.Descendants("li").Count()).Value;
				}

				var pagination = div.Single(x => x.GetAttributeValue("class", null) == "pagination");
				var vals = pagination.Descendants("a").Select(x => Convert.ToInt32(x.GetAttributeValue("data-offset", null)));
				return (_FavoritesCount = vals.Max()).Value;
			}
			catch (WebException e)
			{
				e.Write();
				return -1;
			}
			finally
			{
				resp?.Dispose();
				s?.Dispose();
			}
		}
		private int? _FavoritesCount;
	}
}
