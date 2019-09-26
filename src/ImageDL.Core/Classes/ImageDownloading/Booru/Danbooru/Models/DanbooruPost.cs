using System;
using System.Collections.Generic;
using System.Linq;

using ImageDL.Classes.ImageDownloading.Booru.Models;
using ImageDL.Enums;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Booru.Danbooru.Models
{
	/// <summary>
	/// Json model for a Danbooru post.
	/// </summary>
	public sealed class DanbooruPost : BooruPost
	{
		[JsonProperty("created_at")]
		private DateTime _CreatedAt = default;

		[JsonProperty("image_height")]
		private int _ImageHeight = -1;

		[JsonProperty("image_width")]
		private int _ImageWidth = -1;

		[JsonProperty("tag_string")]
		private string _TagString = null;

		/// <summary>
		/// The id of the person who approved the file.
		/// </summary>
		[JsonProperty("approver_id")]
		public int? ApproverId { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override Uri BaseUrl => new Uri("https://danbooru.donmai.us");

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("bit_flags")]
		public ulong BitFlags { get; private set; }

		/// <summary>
		/// All of the children ids as ints.
		/// </summary>
		[JsonIgnore]
		public int[] ChildrenIds
		{
			get
			{
				if (string.IsNullOrWhiteSpace(ChildrenIdsString))
				{
					return Array.Empty<int>();
				}
				return ChildrenIdsString.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
			}
		}

		/// <summary>
		/// All of the ids of the children.
		/// </summary>
		[JsonProperty("children_ids")]
		public string ChildrenIdsString { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override DateTime CreatedAt => _CreatedAt;

		/// <summary>
		/// The amount of downvotes the post has.
		/// </summary>
		[JsonProperty("down_score")]
		public int DownScore { get; private set; }

		/// <summary>
		/// The amount of favorites the post has.
		/// </summary>
		[JsonProperty("fav_count")]
		public int FavCount { get; private set; }

		/// <summary>
		/// All of the favorites as ints.
		/// </summary>
		[JsonIgnore]
		public int[] Favorites => string.IsNullOrWhiteSpace(FavString)
			? new int[0] : FavString.Split(' ').Select(x => Convert.ToInt32(x.Replace("fav:", ""))).ToArray();

		/// <summary>
		/// Everyone who has favorited the post.
		/// </summary>
		[JsonProperty("fav_string")]
		public string FavString { get; private set; }

		/// <summary>
		/// The file extension.
		/// </summary>
		[JsonProperty("file_ext")]
		public string FileExt { get; private set; }

		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("file_size")]
		public long FileSize { get; private set; }

		/// <summary>
		/// Whether the post has any active children.
		/// </summary>
		[JsonProperty("has_active_children")]
		public bool HasActiveChildren { get; private set; }

		/// <summary>
		/// Whether the post has a large file.
		/// </summary>
		[JsonProperty("has_large")]
		public bool HasLarge { get; private set; }

		/// <summary>
		/// Whether the post has any public children.
		/// </summary>
		[JsonProperty("has_visible_children")]
		public bool HasVisibleChildren { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override int Height => _ImageHeight;

		/// <summary>
		/// Whether the artist is banned.
		/// </summary>
		[JsonProperty("is_banned")]
		public bool IsBanned { get; private set; }

		/// <summary>
		/// Whether the post has been deleted.
		/// </summary>
		[JsonProperty("is_deleted")]
		public bool IsDeleted { get; private set; }

		/// <summary>
		/// Whether the post has been flagged for review by a moderator.
		/// </summary>
		[JsonProperty("is_flagged")]
		public bool IsFlagged { get; private set; }

		/// <summary>
		/// Whether people can add notes.
		/// </summary>
		[JsonProperty("is_note_locked")]
		public bool IsNoteLocked { get; private set; }

		/// <summary>
		/// Whether the post hasn't been approved by a moderator yet.
		/// </summary>
		[JsonProperty("is_pending")]
		public bool IsPending { get; private set; }

		/// <summary>
		/// Whether people can change the rating.
		/// </summary>
		[JsonProperty("is_rating_locked")]
		public bool IsRatingLocked { get; private set; }

		/// <summary>
		/// Whether people can change the status.
		/// </summary>
		[JsonProperty("is_status_locked")]
		public bool IsStatusLocked { get; private set; }

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("keeper_data"), JsonConverter(typeof(KeeperDataConverter))]
		public Dictionary<string, int> KeeperData { get; private set; }

		/// <summary>
		/// The link to the bigger thumbnail url.
		/// </summary>
		[JsonProperty("large_file_url")]
		public string LargeFileUrl { get; private set; }

		/// <summary>
		/// When the last comment was bumped.
		/// </summary>
		[JsonProperty("last_comment_bumped_at")]
		public DateTime? LastCommentBumpedAt { get; private set; }

		/// <summary>
		/// When the last comment was made on the post.
		/// </summary>
		[JsonProperty("last_commented_at")]
		public DateTime? LastCommentedAt { get; private set; }

		/// <summary>
		/// When the last note was made on the post.
		/// </summary>
		[JsonProperty("last_noted_at")]
		public DateTime? LastNotedAt { get; private set; }

		/// <summary>
		/// The id of the pixiv blog the post was taken from.
		/// </summary>
		[JsonProperty("pixiv_id")]
		public int? PixivId { get; private set; }

		/// <summary>
		/// All of the pool names.
		/// </summary>
		[JsonIgnore]
		public string[] Pools => string.IsNullOrWhiteSpace(PoolString)
			? new string[0] : PoolString.Split(' ').Select(x => x.Replace("pool:", "")).ToArray();

		/// <summary>
		/// All of the pools the post is in.
		/// </summary>
		[JsonProperty("pool_string")]
		public string PoolString { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override Uri PostUrl => new Uri($"{BaseUrl}posts/{Id}");

		/// <summary>
		/// The link to the thumbnail url.
		/// </summary>
		[JsonProperty("preview_file_url")]
		public string PreviewFileUrl { get; private set; }

		/// <summary>
		/// The amount of total tags on the post.
		/// </summary>
		[JsonProperty("tag_count")]
		public int TagCount { get; private set; }

		/// <summary>
		/// The amount of arist tags on the post.
		/// </summary>
		[JsonProperty("tag_count_artist")]
		public int TagCountArtist { get; private set; }

		/// <summary>
		/// The amount of character tags on the post.
		/// </summary>
		[JsonProperty("tag_count_character")]
		public int TagCountCharacter { get; private set; }

		/// <summary>
		/// The amount of copyright tags on the post.
		/// </summary>
		[JsonProperty("tag_count_copyright")]
		public int TagCountCopyright { get; private set; }

		/// <summary>
		/// The amount of general tags on the post.
		/// </summary>
		[JsonProperty("tag_count_general")]
		public int TagCountGeneral { get; private set; }

		/// <summary>
		/// The amount of meta tags on the post.
		/// </summary>
		[JsonProperty("tag_count_meta")]
		public int TagCountMeta { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override string Tags => _TagString;

		/// <summary>
		/// All of the artist tags.
		/// </summary>
		[JsonProperty("tag_string_artist")]
		public string TagStringArtist { get; private set; }

		/// <summary>
		/// All of the character tags.
		/// </summary>
		[JsonProperty("tag_string_character")]
		public string TagStringCharacter { get; private set; }

		/// <summary>
		/// All of the copyright tags.
		/// </summary>
		[JsonProperty("tag_string_copyright")]
		public string TagStringCopyright { get; private set; }

		/// <summary>
		/// All of the general tags.
		/// </summary>
		[JsonProperty("tag_string_general")]
		public string TagStringGeneral { get; private set; }

		/// <summary>
		/// All of the meta tags.
		/// </summary>
		[JsonProperty("tag_string_meta")]
		public string TagStringMeta { get; private set; }

		/// <summary>
		/// When the post was last updated.
		/// </summary>
		[JsonProperty("updated_at")]
		public DateTime? UpdatedAt { get; private set; }

		/// <summary>
		/// The id of the person who uploaded the file.
		/// </summary>
		[JsonProperty("uploader_id")]
		public int UploaderId { get; private set; }

		/// <summary>
		/// The name of the person who uploaded the file.
		/// </summary>
		[JsonProperty("uploader_name")]
		public string UploaderName { get; private set; }

		/// <summary>
		/// The amount of upvotes the post has.
		/// </summary>
		[JsonProperty("up_score")]
		public int UpScore { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public override int Width => _ImageWidth;

		/// <summary>
		/// Gets tags for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string[] this[TagType type] => type switch
		{
			TagType.All => Tags.Split(' '),
			TagType.General => TagStringGeneral.Split(' '),
			TagType.Character => TagStringCharacter.Split(' '),
			TagType.Copyright => TagStringCopyright.Split(' '),
			TagType.Artist => TagStringArtist.Split(' '),
			TagType.Meta => TagStringMeta.Split(' '),
			_ => throw new ArgumentOutOfRangeException("Invalid type tag type supplied.", nameof(type)),
		};
	}
}