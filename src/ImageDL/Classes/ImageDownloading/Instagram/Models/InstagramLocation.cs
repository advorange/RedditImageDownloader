using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about where a post was taken.
	/// </summary>
	public sealed class InstagramLocation
	{
		/// <summary>
		/// The id of the location.
		/// </summary>
		[JsonProperty("id")]
		public readonly string Id;
		/// <summary>
		/// Whether the location has a page dedicated to it.
		/// </summary>
		[JsonProperty("has_public_page")]
		public readonly bool HasPublicPage;
		/// <summary>
		/// The name of the location.
		/// </summary>
		[JsonProperty("name")]
		public readonly string Name;
		/// <summary>
		/// The part of the url used to get to the page dedicated to this location.
		/// </summary>
		[JsonProperty("slug")]
		public readonly string Slug;

		/// <summary>
		/// Returns the name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name} ({Id})";
		}
	}
}