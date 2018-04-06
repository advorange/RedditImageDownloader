using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Holds the information for a tag on an Imgur post.
	/// </summary>
	public struct ImgurTag
	{
		/// <summary>
		/// The name of the tag.
		/// </summary>
		[JsonProperty("name")]
		public readonly string Name;
		/// <summary>
		/// How the name is shown to people browser the site.
		/// </summary>
		[JsonProperty("display_name")]
		public readonly string DisplayName;
		/// <summary>
		/// How many people follow this tag.
		/// </summary>
		[JsonProperty("followers")]
		public readonly int Followers;
		/// <summary>
		/// How many items have this tag.
		/// </summary>
		[JsonProperty("total_items")]
		public readonly int TotalItems;
		/// <summary>
		/// If you are following this tag. Will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("following")]
		public readonly bool IsFollowing;
		/// <summary>
		/// The id of the image used for the background.
		/// </summary>
		[JsonProperty("background_hash")]
		public readonly string BackgroundHash;
		/// <summary>
		/// The id of the image used for the thumbnail.
		/// </summary>
		[JsonProperty("thumbnail_hash")]
		public readonly string ThumbnailHash;
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("accent")]
		public readonly string Accent;
		/// <summary>
		/// Whether the background is animated.
		/// </summary>
		[JsonProperty("background_is_animated")]
		public readonly bool IsBackgroundAnimated;
		/// <summary>
		/// Whether the thumbnail is animated.
		/// </summary>
		[JsonProperty("thumbnail_is_animated")]
		public readonly bool IsThumbnailAnimated;
		/// <summary>
		/// Whether the tag is promoted by a company for advertisement.
		/// </summary>
		[JsonProperty("is_promoted")]
		public readonly bool IsPromoted;
		/// <summary>
		/// A short description of the tag.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;
		/// <summary>
		/// The id of the image used for the logo.
		/// </summary>
		[JsonProperty("logo_hash")]
		public readonly string LogoHash;
		/// <summary>
		/// The url 
		/// </summary>
		[JsonProperty("logo_destination_url")]
		public readonly string LogoDestinationUrl;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("description_annotations")]
		public readonly object DescriptionAnnotations; //Not sure what type they are

		/// <summary>
		/// Returns the name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}
}