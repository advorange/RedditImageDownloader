using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Imgur
{
#pragma warning disable 1591, 649 //Disabled since most of these are self explanatory and this is a glorified Json model
	/// <summary>
	/// Json model for a post from Imgur. Inherits from <see cref="ImgurImage"/> because there is a chance the post isn't an album.
	/// </summary>
	public sealed class ImgurPost : ImgurImage
	{
		[JsonProperty("cover")]
		public readonly string Cover;
		[JsonProperty("cover_width")]
		public readonly int CoverWidth;
		[JsonProperty("cover_height")]
		public readonly int CoverHeight;
		[JsonProperty("privacy")]
		public readonly string Privacy;
		[JsonProperty("layout")]
		public readonly string Layout;
		[JsonProperty("topic")]
		public readonly string Topic;
		[JsonProperty("topic_id")]
		public readonly int TopicId;
		[JsonProperty("is_album")]
		public readonly bool IsAlbum;
		[JsonProperty("is_gallery")]
		public readonly bool IsGallery;
		[JsonProperty("images_count")]
		public readonly int ImagesCount;
		[JsonProperty("images")]
		private List<ImgurImage> _Images;

		[JsonIgnore]
		public List<ImgurImage> Images => _Images ?? (_Images = new List<ImgurImage>() { this, });
	}

	/// <summary>
	/// Json model for an image from Imgur.
	/// </summary>
	public class ImgurImage : ImgurThing
	{
		[JsonProperty("type")]
		public readonly string Type;
		[JsonProperty("animated")]
		public readonly bool IsAnimated;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("size")]
		public readonly long FileSize;
		[JsonProperty("bandwidth")]
		public readonly long BandWidth;
		[JsonProperty("in_gallery")]
		public readonly bool IsInGallery;
		[JsonProperty("has_sound")]
		public readonly bool HasSound;
	}

	/// <summary>
	/// Json model for a post/image from Imgur.
	/// </summary>
	public abstract class ImgurThing
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("title")]
		public readonly string Title;
		[JsonProperty("description")]
		public readonly string Description;
		[JsonProperty("views")]
		public readonly int Views;
		[JsonProperty("vote")]
		public readonly string Vote;
		[JsonProperty("ups")]
		public readonly int? UpScore;
		[JsonProperty("downs")]
		public readonly int? DownScore;
		[JsonProperty("score")]
		public readonly int? Score;
		[JsonProperty("points")]
		public readonly int? Points;
		[JsonProperty("comment_count")]
		public readonly int? CommentCount;
		[JsonProperty("favorite_count")]
		public readonly int? FavoriteCount;
		[JsonProperty("tags")]
		public readonly List<Tag> Tags;
		[JsonProperty("is_ad")]
		public readonly bool IsAd;
		[JsonProperty("ad_type")]
		public readonly int AdType;
		[JsonProperty("ad_url")]
		public readonly string AdUrl;
		[JsonProperty("in_most_viral")]
		public readonly bool IsInMostViral;
		[JsonProperty("account_url")]
		public readonly string AccountName;
		[JsonProperty("account_id")]
		public readonly int? AccountId;
		[JsonProperty("favorite")]
		public readonly bool IsFavorited;
		[JsonProperty("nsfw")]
		public readonly bool? IsNSFW;
		[JsonProperty("section")]
		public readonly string Section;
		[JsonProperty("mp4_size")]
		public readonly long Mp4Size;
		[JsonProperty("looping")]
		public readonly bool IsLooping;
		[JsonProperty("mp4")]
		public readonly string Mp4Link;
		[JsonProperty("gifv")]
		public readonly string GifvLink;
		[JsonProperty("link")]
		public readonly string Link;
		[JsonProperty("datetime")]
		private readonly long _DateTime;

		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(_DateTime)).ToUniversalTime();
		[JsonIgnore]
		public string ImageLink => Mp4Link ?? Link;

		public override string ToString()
		{
			return Id;
		}
	}

	/// <summary>
	/// Holds the information for a tag on an Imgur post.
	/// </summary>
	public struct Tag
	{
		[JsonProperty("name")]
		public readonly string Name;
		[JsonProperty("display_name")]
		public readonly string DisplayName;
		[JsonProperty("followers")]
		public readonly int Followers;
		[JsonProperty("total_items")]
		public readonly int TotalItems;
		[JsonProperty("following")]
		public readonly bool IsFollowing;
		[JsonProperty("background_hash")]
		public readonly string BackgroundHash;
		[JsonProperty("thumbnail_hash")]
		public readonly string ThumbnailHash;
		[JsonProperty("accent")]
		public readonly string Accent;
		[JsonProperty("background_is_animated")]
		public readonly bool IsBackgroundAnimated;
		[JsonProperty("thumbnail_is_animated")]
		public readonly bool IsThumbnailAnimated;
		[JsonProperty("is_promoted")]
		public readonly bool IsPromoted;
		[JsonProperty("description")]
		public readonly string Description;
		[JsonProperty("logo_hash")]
		public readonly string LogoHash;
		[JsonProperty("logo_destination_url")]
		public readonly string LogoDestinationUrl;
		[JsonProperty("description_annotations")]
		public readonly object DescriptionAnnotations; //Not sure what type they are
	}
#pragma warning restore 1591, 649
}
