#pragma warning disable 1591, 649
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for a post/image from Imgur.
	/// </summary>
	public abstract class ImgurThing : Post
	{
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
		[JsonProperty("points")]
		public readonly int? Points;
		[JsonProperty("comment_count")]
		public readonly int? CommentCount;
		[JsonProperty("favorite_count")]
		public readonly int? FavoriteCount;
		[JsonProperty("tags")]
		public readonly List<ImgurTag> Tags;
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

		[JsonProperty("datetime")]
		private readonly long _DateTime;
		[JsonProperty("link")]
		private readonly string _Link;
		[JsonProperty("id")]
		private readonly string _Id;
		[JsonProperty("score")]
		private readonly int? _Score;

		[JsonIgnore]
		public override string PostUrl => _Link;
		[JsonIgnore]
		public override string ContentUrl => Mp4Link ?? PostUrl;
		[JsonIgnore]
		public override string Id => _Id;
		[JsonIgnore]
		public override int Score => _Score ?? 0;
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(_DateTime)).ToUniversalTime();

		/// <inheritdoc />
		public override string ToString()
		{
			return Id;
		}
	}
}
