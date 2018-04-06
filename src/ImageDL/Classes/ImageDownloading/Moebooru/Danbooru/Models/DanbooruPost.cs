using System;
using System.Collections.Generic;
using System.Linq;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using ImageDL.Enums;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Danbooru.Models
{
	/// <summary>
	/// Json model for a Danbooru post.
	/// </summary>
	public sealed class DanbooruPost : MoebooruPost
	{
		#region Json
		/// <summary>
		/// The id of the person who uploaded the file.
		/// </summary>
		[JsonProperty("uploader_id")]
		public readonly int UploaderId;
		/// <summary>
		/// The id of the person who approved the file.
		/// </summary>
		[JsonProperty("approver_id")]
		public readonly int? ApproverId;
		/// <summary>
		/// The id of the pixiv blog the post was taken from.
		/// </summary>
		[JsonProperty("pixiv_id")]
		public readonly int? PixivId;
		/// <summary>
		/// The amount of upvotes the post has.
		/// </summary>
		[JsonProperty("up_score")]
		public readonly int UpScore;
		/// <summary>
		/// The amount of downvotes the post has.
		/// </summary>
		[JsonProperty("down_score")]
		public readonly int DownScore;
		/// <summary>
		/// The amount of favorites the post has.
		/// </summary>
		[JsonProperty("fav_count")]
		public readonly int FavCount;
		/// <summary>
		/// Whether people can add notes.
		/// </summary>
		[JsonProperty("is_note_locked")]
		public readonly bool IsNoteLocked;
		/// <summary>
		/// Whether people can change the rating.
		/// </summary>
		[JsonProperty("is_rating_locked")]
		public readonly bool IsRatingLocked;
		/// <summary>
		/// Whether people can change the status.
		/// </summary>
		[JsonProperty("is_status_locked")]
		public readonly bool IsStatusLocked;
		/// <summary>
		/// Whether the post hasn't been approved by a moderator yet.
		/// </summary>
		[JsonProperty("is_pending")]
		public readonly bool IsPending;
		/// <summary>
		/// Whether the post has been flagged for review by a moderator.
		/// </summary>
		[JsonProperty("is_flagged")]
		public readonly bool IsFlagged;
		/// <summary>
		/// Whether the post has been deleted.
		/// </summary>
		[JsonProperty("is_deleted")]
		public readonly bool IsDeleted;
		/// <summary>
		/// Whether the artist is banned.
		/// </summary>
		[JsonProperty("is_banned")]
		public readonly bool IsBanned;
		/// <summary>
		/// The file extension.
		/// </summary>
		[JsonProperty("file_ext")]
		public readonly string FileExt;
		/// <summary>
		/// The link to the bigger thumbnail url.
		/// </summary>
		[JsonProperty("large_file_url")]
		public readonly string LargeFileUrl;
		/// <summary>
		/// The link to the thumbnail url.
		/// </summary>
		[JsonProperty("preview_file_url")]
		public readonly string PreviewFileUrl;
		/// <summary>
		/// Whether the post has a large file.
		/// </summary>
		[JsonProperty("has_large")]
		public readonly bool HasLarge;
		/// <summary>
		/// Whether the post has any active children.
		/// </summary>
		[JsonProperty("has_active_children")]
		public readonly bool HasActiveChildren;
		/// <summary>
		/// Whether the post has any public children.
		/// </summary>
		[JsonProperty("has_visible_children")]
		public readonly bool HasVisibleChildren;
		/// <summary>
		/// All of the ids of the children.
		/// </summary>
		[JsonProperty("children_ids")]
		public readonly string ChildrenIdsString;
		/// <summary>
		/// When the last comment was bumped.
		/// </summary>
		[JsonProperty("last_comment_bumped_at")]
		public readonly DateTime? LastCommentBumpedAt;
		/// <summary>
		/// When the last note was made on the post.
		/// </summary>
		[JsonProperty("last_noted_at")]
		public readonly DateTime? LastNotedAt;
		/// <summary>
		/// When the post was last updated.
		/// </summary>
		[JsonProperty("updated_at")]
		public readonly DateTime? UpdatedAt;
		/// <summary>
		/// When the last comment was made on the post.
		/// </summary>
		[JsonProperty("last_commented_at")]
		public readonly DateTime? LastCommentedAt;
		/// <summary>
		/// The name of the person who uploaded the file.
		/// </summary>
		[JsonProperty("uploader_name")]
		public readonly string UploaderName;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("bit_flags")]
		public readonly ulong BitFlags; //Not sure if this is the correct type
		/// <summary>
		/// Everyone who has favorited the post.
		/// </summary>
		[JsonProperty("fav_string")]
		public readonly string FavString;
		/// <summary>
		/// All of the pools the post is in.
		/// </summary>
		[JsonProperty("pool_string")]
		public readonly string PoolString;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("keeper_data")]
		public readonly Dictionary<string, int> KeeperData; //Not sure if this is the correct type
		/// <summary>
		/// All of the tags.
		/// </summary>
		[JsonProperty("tag_string")]
		public readonly string TagString;
		/// <summary>
		/// All of the general tags.
		/// </summary>
		[JsonProperty("tag_string_general")]
		public readonly string TagStringGeneral;
		/// <summary>
		/// All of the character tags.
		/// </summary>
		[JsonProperty("tag_string_character")]
		public readonly string TagStringCharacter;
		/// <summary>
		/// All of the copyright tags.
		/// </summary>
		[JsonProperty("tag_string_copyright")]
		public readonly string TagStringCopyright;
		/// <summary>
		/// All of the artist tags.
		/// </summary>
		[JsonProperty("tag_string_artist")]
		public readonly string TagStringArtist;
		/// <summary>
		/// All of the meta tags.
		/// </summary>
		[JsonProperty("tag_string_meta")]
		public readonly string TagStringMeta;
		/// <summary>
		/// The amount of total tags on the post.
		/// </summary>
		[JsonProperty("tag_count")]
		public readonly int TagCount;
		/// <summary>
		/// The amount of general tags on the post.
		/// </summary>
		[JsonProperty("tag_count_general")]
		public readonly int TagCountGeneral;
		/// <summary>
		/// The amount of character tags on the post.
		/// </summary>
		[JsonProperty("tag_count_character")]
		public readonly int TagCountCharacter;
		/// <summary>
		/// The amount of copyright tags on the post.
		/// </summary>
		[JsonProperty("tag_count_copyright")]
		public readonly int TagCountCopyright;
		/// <summary>
		/// The amount of arist tags on the post.
		/// </summary>
		[JsonProperty("tag_count_artist")]
		public readonly int TagCountArtist;
		/// <summary>
		/// The amount of meta tags on the post.
		/// </summary>
		[JsonProperty("tag_count_meta")]
		public readonly int TagCountMeta;
		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonProperty("file_size")]
		public readonly long FileSize;
		[JsonProperty("image_width")]
		private readonly int _Width = -1;
		[JsonProperty("image_height")]
		private readonly int _Height = -1;
		[JsonProperty("created_at")]
		private readonly DateTime _CreatedAt = new DateTime(1970, 1, 1);
		#endregion

		/// <inheritdoc />
		public override Uri BaseUrl => new Uri("https://danbooru.donmai.us");
		/// <inheritdoc />
		public override Uri PostUrl => new Uri($"{BaseUrl}/posts/{Id}");
		/// <inheritdoc />
		public override DateTime CreatedAt => _CreatedAt.ToUniversalTime();
		/// <inheritdoc />
		public override int Width => _Width;
		/// <inheritdoc />
		public override int Height => _Height;
		/// <inheritdoc />
		public override string Tags => TagString;

		/// <summary>
		/// Gets tags for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
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
		/// <summary>
		/// All of the children ids as ints.
		/// </summary>
		public int[] ChildrenIds => String.IsNullOrWhiteSpace(ChildrenIdsString)
			? new int[0] : ChildrenIdsString.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
		/// <summary>
		/// All of the favorites as ints.
		/// </summary>
		public int[] Favorites => String.IsNullOrWhiteSpace(FavString)
			? new int[0] : FavString.Split(' ').Select(x => Convert.ToInt32(x.Replace("fav:", ""))).ToArray();
		/// <summary>
		/// All of the pool names.
		/// </summary>
		public string[] Pools => String.IsNullOrWhiteSpace(PoolString)
			? new string[0] : PoolString.Split(' ').Select(x => x.Replace("pool:", "")).ToArray();
	}
}