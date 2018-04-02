#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Information about where a post was taken.
	/// </summary>
	public sealed class Location
	{
		[JsonProperty("id")]
		public readonly string Id;
		[JsonProperty("has_public_page")]
		public readonly bool HasPublicPage;
		[JsonProperty("name")]
		public readonly string Name;
		[JsonProperty("slug")]
		public readonly string Slug;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Name} ({Id})";
		}
	}
}
