#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for a user's Vsco page.
	/// </summary>
	public class VscoUserInfo
	{
		[JsonProperty("collection_share_link")]
		public readonly string CollectionShareLink;
		[JsonProperty("description")]
		public readonly string Description;
		[JsonProperty("domain")]
		public readonly string Domain;
		[JsonProperty("externalLink")]
		public readonly string ExternalLink;
		[JsonProperty("externalLinkDisplayText")]
		public readonly string ExternalLinkDisplayText;
		[JsonProperty("grid_album_id")]
		public readonly string GridAlbumId;
		[JsonProperty("has_article")]
		public readonly bool HasArticle;
		[JsonProperty("has_collection")]
		public readonly bool HasCollection;
		[JsonProperty("has_grid")]
		public readonly bool HasGrid;
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("is_brand")]
		public readonly bool IsBrand;
		[JsonProperty("internal_site")]
		public readonly bool InternalSite;
		[JsonProperty("museum")]
		public readonly bool Museum;
		[JsonProperty("name")]
		public readonly string Name;
		[JsonProperty("password")]
		public readonly object Password;
		[JsonProperty("profile_image")]
		public readonly string ProfileImage;
		[JsonProperty("profile_image_id")]
		public readonly string ProfileImageId;
		[JsonProperty("recently_published")]
		public readonly string RecentlyPublished;
		[JsonProperty("responsive_url")]
		public readonly string ResponsiveUrl;
		[JsonProperty("share_link")]
		public readonly string ShareLink;
		[JsonProperty("site_collection_id")]
		public readonly string SiteCollectionId;
		[JsonProperty("status")]
		public readonly string Status;
		[JsonProperty("subdomain")]
		public readonly string Subdomain;
		[JsonProperty("type")]
		public readonly int Type;
		[JsonProperty("user_id")]
		public readonly int UserId;
	}
}
