#pragma warning disable 1591, 649
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Holds the information for a tag on an Imgur post.
	/// </summary>
	public struct ImgurTag
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

		/// <inheritdoc />
		public override string ToString()
		{
			return Name;
		}
	}
}
