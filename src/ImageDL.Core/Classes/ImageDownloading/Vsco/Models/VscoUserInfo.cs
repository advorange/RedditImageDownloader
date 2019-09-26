using System;

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
		public string CollectionShareLink { get; private set; }

		/// <summary>
		/// Description of the user's page.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }

		/// <summary>
		/// The domain leading to the user's page.
		/// </summary>
		[JsonProperty("domain")]
		public string Domain { get; private set; }

		/// <summary>
		/// Link to a different website.
		/// </summary>
		[JsonProperty("externalLink")]
		public string ExternalLink { get; private set; }

		/// <summary>
		/// The text displayed for the external link.
		/// </summary>
		[JsonProperty("externalLinkDisplayText")]
		public string ExternalLinkDisplayText { get; private set; }

		/// <summary>
		/// Hash id of the users's gallery.
		/// </summary>
		[JsonProperty("grid_album_id")]
		public string GridAlbumId { get; private set; }

		/// <summary>
		/// User has journal entries.
		/// </summary>
		[JsonProperty("has_article")]
		public bool HasArticle { get; private set; }

		/// <summary>
		/// User has collection, which is sharing other people's images.
		/// </summary>
		[JsonProperty("has_collection")]
		public bool HasCollection { get; private set; }

		/// <summary>
		/// User has the standard image grid.
		/// </summary>
		[JsonProperty("has_grid")]
		public bool HasGrid { get; private set; }

		/// <summary>
		/// The page's id.
		/// </summary>
		[JsonProperty("id")]
		public ulong Id { get; private set; }

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("internal_site")]
		public bool InternalSite { get; private set; }

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("is_brand")]
		public bool IsBrand { get; private set; }

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("museum")]
		public bool Museum { get; private set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// This is kind of weird this is a field. No clue what it does. Hopefully this isn't anything sensitive.
		/// </summary>
		[JsonProperty("password")]
		public object Password { get; private set; }

		/// <summary>
		/// Link to the user's profile picture.
		/// </summary>
		[JsonProperty("profile_image")]
		public string ProfileImage { get; private set; }

		/// <summary>
		/// Hash of the profile picture.
		/// </summary>
		[JsonProperty("profile_image_id")]
		public string ProfileImageId { get; private set; }

		/// <summary>
		/// Most recently published image.
		/// </summary>
		[JsonProperty("recently_published")]
		public string RecentlyPublished { get; private set; }

		/// <summary>
		/// Short host url.
		/// </summary>
		[JsonProperty("responsive_url")]
		public string ResponsiveUrl { get; private set; }

		/// <summary>
		/// Link to share the gallery.
		/// </summary>
		[JsonProperty("share_link")]
		public Uri ShareLink { get; private set; }

		/// <summary>
		/// Hash id of the site collection.
		/// </summary>
		[JsonProperty("site_collection_id")]
		public string SiteCollectionId { get; private set; }

		/// <summary>
		/// The collection's status, e.g. draft, published, etc.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; private set; }

		/// <summary>
		/// Generally the user's username again.
		/// </summary>
		[JsonProperty("subdomain")]
		public string Subdomain { get; private set; }

		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("type")]
		public int Type { get; private set; }

		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("user_id")]
		public int UserId { get; private set; }
	}
}