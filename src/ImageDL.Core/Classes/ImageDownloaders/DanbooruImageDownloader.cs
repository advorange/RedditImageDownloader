using ImageDL.Classes.ImageGatherers;
using ImageDL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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
				NotifyPropertyChanged(_TagString = value);
			}
		}
		/// <summary>
		/// The page to start downloading from.
		/// </summary>
		public int Page
		{
			get => _Page;
			set => NotifyPropertyChanged(_Page = Math.Min(1, value));
		}

		private HttpClient _Client;
		private string _TagString;
		private int _Page;

		public DanbooruImageDownloader()
		{
			CommandLineParserOptions.Add($"tags|{nameof(TagString)}=", "the tags to search for.", i => SetValue<string>(i, c => TagString = c));
			CommandLineParserOptions.Add($"{nameof(Page)}=", "the page to start from.", i => SetValue<int>(i, c => Page = c));

			Page = 1;

			_Client = new HttpClient();
		}

		/// <inheritdoc />
		protected override async Task<List<DanbooruPost>> GatherPostsAsync()
		{
			var validPosts = new List<DanbooruPost>();
			try
			{
				//Uses for instead of while to save 2 lines.
				var nextRetry = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
				for (int i = 0; validPosts.Count < AmountToDownload; ++i)
				{
					var diff = nextRetry - DateTime.UtcNow;
					if (diff.Ticks > 0)
					{
						await Task.Delay(diff).ConfigureAwait(false);
					}

					try
					{
						var iterVal = await IterateAsync(validPosts, i).ConfigureAwait(false);
						if (iterVal < 0)
						{
							break;
						}
					}
					catch (HttpRequestException hre) when (hre.Message.Contains("421")) //Rate limited
					{
						nextRetry = DateTime.UtcNow.AddSeconds(30);
						Console.WriteLine($"Rate limited; retrying next at: {nextRetry.ToLongTimeString()}");
						--i; //To make up for how this rate limited request doesn't return anything
						continue;
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
		protected override FileInfo GenerateFileInfo(DanbooruPost post, WebResponse response, Uri uri)
		{
			var totalName = $"{post.Id}_{(response.ResponseUri.LocalPath ?? uri.ToString()).Split('/').Last()}";
			var validName = new string(totalName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
			return new FileInfo(Path.Combine(Directory, validName));
		}
		/// <inheritdoc />
		protected override async Task<ImageGatherer> CreateGathererAsync(DanbooruPost post)
		{
			var uri = new Uri($"http://danbooru.donmai.us{post.FileUrl}");
			return await ImageGatherer.CreateGathererAsync(Scrapers, uri).ConfigureAwait(false);
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DanbooruPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}

		private async Task<int> IterateAsync(List<DanbooruPost> validPosts, int iteration)
		{
			//Limit caps out at 200 per pages, so can't get this all in one iteration. Have to keep incrementing page.
			var search = $"https://danbooru.donmai.us/posts.json?utf8=✓" +
				$"&tags={WebUtility.UrlEncode(TagString)}" +
				$"&limit={AmountToDownload}" +
				$"&page={Page + iteration}";

			//Get the Json from Danbooru
			string json;
			using (var resp = await _Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, search)).ConfigureAwait(false))
			{
				resp.EnsureSuccessStatusCode();
				json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
			}

			//Deserialize the Json and look through all the posts
			var posts = JsonConvert.DeserializeObject<List<DanbooruPost>>(json);
			foreach (var post in posts)
			{
				if (post.CreatedAt.ToUniversalTime() < OldestAllowed)
				{
					return -1;
				}
				else if (post.ImageWidth < MinWidth || post.ImageHeight < MinHeight || post.Score < MinScore)
				{
					continue;
				}

				validPosts.Add(post);
				if (validPosts.Count == AmountToDownload)
				{
					return -1;
				}
				else if (validPosts.Count % 25 == 0)
				{
					Console.WriteLine($"{validPosts.Count} Danbooru posts found.");
				}
			}

			//Anything less than a full page means everything's been searched
			return posts.Count < Math.Min(AmountToDownload, 200) ? -1 : posts.Count;
		}
	}

	/// <summary>
	/// Json model for a danbooru post.
	/// </summary>
	public class DanbooruPost
	{
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("uploader_id")]
		public readonly int UploaderId;
		[JsonProperty("parent_id")]
		public readonly int? ParentId;
		[JsonProperty("approver_id")]
		public readonly int? ApproverId;
		[JsonProperty("pixiv_id")]
		public readonly int? PixivId;

		[JsonProperty("score")]
		public readonly int Score;
		[JsonProperty("up_score")]
		public readonly int UpScore;
		[JsonProperty("down_score")]
		public readonly int DownScore;
		[JsonProperty("fav_count")]
		public readonly int FavCount;

		[JsonProperty("is_note_locked")]
		public readonly bool IsNoteLocked;
		[JsonProperty("is_rating_locked")]
		public readonly bool IsRatingLocked;
		[JsonProperty("is_status_locked")]
		public readonly bool IsStatusLocked;
		[JsonProperty("is_pending")]
		public readonly bool IsPending;
		[JsonProperty("is_flagged")]
		public readonly bool IsFlagged;
		[JsonProperty("is_deleted")]
		public readonly bool IsDeleted;
		[JsonProperty("is_banned")]
		public readonly bool IsBanned;

		[JsonProperty("md5")]
		public readonly string Md5;
		[JsonProperty("file_ext")]
		public readonly string FileExt;
		[JsonProperty("file_size")]
		public readonly long FileSize;
		[JsonProperty("image_width")]
		public readonly int ImageWidth;
		[JsonProperty("image_height")]
		public readonly int ImageHeight;
		[JsonProperty("file_url")]
		public readonly string FileUrl;
		[JsonProperty("large_file_url")]
		public readonly string LargeFileUrl;
		[JsonProperty("preview_file_url")]
		public readonly string PreviewFileUrl;
		[JsonProperty("has_large")]
		public readonly bool HasLarge;

		[JsonProperty("has_children")]
		public readonly bool HasChildren;
		[JsonProperty("has_active_children")]
		public readonly bool HasActiveChildren;
		[JsonProperty("has_visible_children")]
		public readonly bool HasVisibleChildren;
		[JsonProperty("children_ids")]
		public readonly string ChildrenIdsString;

		[JsonProperty("created_at")]
		public readonly DateTime CreatedAt;
		[JsonProperty("last_comment_bumped_at")]
		public readonly DateTime? LastCommentBumpedAt;
		[JsonProperty("last_noted_at")]
		public readonly DateTime? LastNotedAt;
		[JsonProperty("updated_at")]
		public readonly DateTime? UpdatedAt;
		[JsonProperty("last_commented_at")]
		public readonly DateTime? LastCommentedAt;

		[JsonProperty("source")]
		public readonly string Source;
		[JsonProperty("uploader_name")]
		public readonly string UploaderName;
		[JsonProperty("rating")]
		public readonly char Rating;
		[JsonProperty("bit_flags")]
		public readonly ulong BitFlags; //Not sure if this is the correct type
		[JsonProperty("fav_string")]
		public readonly string FavString;
		[JsonProperty("pool_string")]
		public readonly string PoolString;
		[JsonProperty("keeper_data")]
		public readonly Dictionary<string, int> KeeperData; //Not sure if this is the correct type

		#region Tags
		[JsonProperty("tag_string")]
		public readonly string TagString;
		[JsonProperty("tag_string_general")]
		public readonly string TagStringGeneral;
		[JsonProperty("tag_string_character")]
		public readonly string TagStringCharacter;
		[JsonProperty("tag_string_copyright")]
		public readonly string TagStringCopyright;
		[JsonProperty("tag_string_artist")]
		public readonly string TagStringArtist;
		[JsonProperty("tag_string_meta")]
		public readonly string TagStringMeta;

		[JsonProperty("tag_count")]
		public readonly int TagCount;
		[JsonProperty("tag_count_general")]
		public readonly int TagCountGeneral;
		[JsonProperty("tag_count_character")]
		public readonly int TagCountCharacter;
		[JsonProperty("tag_count_copyright")]
		public readonly int TagCountCopyright;
		[JsonProperty("tag_count_artist")]
		public readonly int TagCountArtist;
		[JsonProperty("tag_count_meta")]
		public readonly int TagCountMeta;
		#endregion

		public string[] this[TagType type]
		{
			get
			{
				switch (type)
				{
					case TagType.All:
						return TagString.Split(' ');
					case TagType.General:
						return TagStringGeneral.Split(' ');
					case TagType.Character:
						return TagStringCharacter.Split(' ');
					case TagType.Copyright:
						return TagStringCopyright.Split(' ');
					case TagType.Artist:
						return TagStringArtist.Split(' ');
					case TagType.Meta:
						return TagStringMeta.Split(' ');
					default:
						throw new ArgumentException("Invalid type tag type supplied.", nameof(type));
				}
			}
		}

		[JsonIgnore]
		public int[] ChildrenIds => String.IsNullOrWhiteSpace(ChildrenIdsString)
			? new int[0] : ChildrenIdsString.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
		[JsonIgnore]
		public int[] Favorites => String.IsNullOrWhiteSpace(FavString)
			? new int[0] : FavString.Split(' ').Select(x => Convert.ToInt32(x.Replace("fav:", ""))).ToArray();
		[JsonIgnore]
		public string[] Pools => String.IsNullOrWhiteSpace(PoolString)
			? new string[0] : PoolString.Split(' ').Select(x => x.Replace("pool:", "")).ToArray();

		public enum TagType
		{
			/// <summary>
			/// Every tag that the image has on it. General, meta, etc.
			/// </summary>
			All,
			/// <summary>
			/// Tags for what the character is doing or looking like.
			/// </summary>
			General,
			/// <summary>
			/// Tags for who is in the image.
			/// </summary>
			Character,
			/// <summary>
			/// Tags for who owns the image.
			/// </summary>
			Copyright,
			/// <summary>
			/// Tags for who made the image.
			/// </summary>
			Artist,
			/// <summary>
			/// Tags about the image file. Resolution, official, etc.
			/// </summary>
			Meta,
		}
	}
}
