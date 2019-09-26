using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about where a post was taken.
	/// </summary>
	public sealed class InstagramLocation
	{
		/// <summary>
		/// Whether the location has a page dedicated to it.
		/// </summary>
		[JsonProperty("has_public_page")]
		public bool HasPublicPage { get; private set; }

		/// <summary>
		/// The id of the location.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// The name of the location.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// The part of the url used to get to the page dedicated to this location.
		/// </summary>
		[JsonProperty("slug")]
		public string Slug { get; private set; }

		/// <summary>
		/// Returns the name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} ({Id})";
	}
}