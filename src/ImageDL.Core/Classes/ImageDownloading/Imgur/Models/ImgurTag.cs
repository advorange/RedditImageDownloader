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
		public string Name { get; private set; }
		/// <summary>
		/// How the name is shown to people browser the site.
		/// </summary>
		[JsonProperty("display_name")]
		public string DisplayName { get; private set; }
		/// <summary>
		/// How many people follow this tag.
		/// </summary>
		[JsonProperty("followers")]
		public int Followers { get; private set; }
		/// <summary>
		/// How many items have this tag.
		/// </summary>
		[JsonProperty("total_items")]
		public int TotalItems { get; private set; }
		/// <summary>
		/// If you are following this tag. Will always be false because we're not logged in.
		/// </summary>
		[JsonProperty("following")]
		public bool IsFollowing { get; private set; }
		/// <summary>
		/// The id of the image used for the background.
		/// </summary>
		[JsonProperty("background_hash")]
		public string BackgroundHash { get; private set; }
		/// <summary>
		/// The id of the image used for the thumbnail.
		/// </summary>
		[JsonProperty("thumbnail_hash")]
		public string ThumbnailHash { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("accent")]
		public string Accent { get; private set; }
		/// <summary>
		/// Whether the background is animated.
		/// </summary>
		[JsonProperty("background_is_animated")]
		public bool IsBackgroundAnimated { get; private set; }
		/// <summary>
		/// Whether the thumbnail is animated.
		/// </summary>
		[JsonProperty("thumbnail_is_animated")]
		public bool IsThumbnailAnimated { get; private set; }
		/// <summary>
		/// Whether the tag is promoted by a company for advertisement.
		/// </summary>
		[JsonProperty("is_promoted")]
		public bool IsPromoted { get; private set; }
		/// <summary>
		/// A short description of the tag.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// The id of the image used for the logo.
		/// </summary>
		[JsonProperty("logo_hash")]
		public string LogoHash { get; private set; }
		/// <summary>
		/// The url 
		/// </summary>
		[JsonProperty("logo_destination_url")]
		public string LogoDestinationUrl { get; private set; }
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("description_annotations")]
		public object DescriptionAnnotations { get; private set; } //Not sure what type they are

		/// <summary>
		/// Returns the name.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Name;
	}
}