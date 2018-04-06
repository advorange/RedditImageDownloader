using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for a user's Vsco page.
	/// </summary>
	public class VscoUserInfo
	{
		/// <summary>
		/// Link to the user's images.
		/// </summary>
		[JsonProperty("collection_share_link")]
		public readonly string CollectionShareLink;
		/// <summary>
		/// Description of the user's page.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;
		/// <summary>
		/// The domain leading to the user's page.
		/// </summary>
		[JsonProperty("domain")]
		public readonly string Domain;
		/// <summary>
		/// Link to a different website.
		/// </summary>
		[JsonProperty("externalLink")]
		public readonly string ExternalLink;
		/// <summary>
		/// The text displayed for the external link.
		/// </summary>
		[JsonProperty("externalLinkDisplayText")]
		public readonly string ExternalLinkDisplayText;
		/// <summary>
		/// Hash id of the users's gallery.
		/// </summary>
		[JsonProperty("grid_album_id")]
		public readonly string GridAlbumId;
		/// <summary>
		/// User has journal entries.
		/// </summary>
		[JsonProperty("has_article")]
		public readonly bool HasArticle;
		/// <summary>
		/// User has collection, which is sharing other people's images.
		/// </summary>
		[JsonProperty("has_collection")]
		public readonly bool HasCollection;
		/// <summary>
		/// User has the standard image grid.
		/// </summary>
		[JsonProperty("has_grid")]
		public readonly bool HasGrid;
		/// <summary>
		/// The page's id.
		/// </summary>
		[JsonProperty("id")]
		public readonly int Id;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("is_brand")]
		public readonly bool IsBrand;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("internal_site")]
		public readonly bool InternalSite;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("museum")]
		public readonly bool Museum;
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("name")]
		public readonly string Name;
		/// <summary>
		/// This is kind of weird this is a field. No clue what it does. Hopefully this isn't anything sensitive.
		/// </summary>
		[JsonProperty("password")]
		public readonly object Password;
		/// <summary>
		/// Link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_image")]
		public readonly string ProfileImage;
		/// <summary>
		/// Hash of the profile picture.
		/// </summary>
		[JsonProperty("profile_image_id")]
		public readonly string ProfileImageId;
		/// <summary>
		/// Most recently published image.
		/// </summary>
		[JsonProperty("recently_published")]
		public readonly string RecentlyPublished;
		/// <summary>
		/// Short host url.
		/// </summary>
		[JsonProperty("responsive_url")]
		public readonly string ResponsiveUrl;
		/// <summary>
		/// Link to share the gallery.
		/// </summary>
		[JsonProperty("share_link")]
		public readonly string ShareLink;
		/// <summary>
		/// Hash id of the site collection.
		/// </summary>
		[JsonProperty("site_collection_id")]
		public readonly string SiteCollectionId;
		/// <summary>
		/// The collection's status, e.g. draft, published, etc.
		/// </summary>
		[JsonProperty("status")]
		public readonly string Status;
		/// <summary>
		/// Generally the user's username again.
		/// </summary>
		[JsonProperty("subdomain")]
		public readonly string Subdomain;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("type")]
		public readonly int Type;
		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("user_id")]
		public readonly int UserId;
	}
}