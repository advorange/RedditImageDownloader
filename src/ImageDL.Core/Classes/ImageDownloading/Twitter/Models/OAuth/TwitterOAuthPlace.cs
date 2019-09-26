using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Named place with coordinates.
	/// </summary>
	public struct TwitterOAuthPlace
	{
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("attributes")]
		public object Attributes { get; private set; }

		/// <summary>
		/// The coordinates of a place.
		/// </summary>
		[JsonProperty("bounding_box")]
		public TwitterOAuthBoundingBox BoundingBox { get; private set; }

		/// <summary>
		/// The country's name.
		/// </summary>
		[JsonProperty("country")]
		public string Country { get; private set; }

		/// <summary>
		/// The country's code.
		/// </summary>
		[JsonProperty("country_code")]
		public string CountryCode { get; private set; }

		/// <summary>
		/// The full name of the place.
		/// </summary>
		[JsonProperty("full_name")]
		public string FullName { get; private set; }

		/// <summary>
		/// Id of the place.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Shorter version of <see cref="FullName"/>.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// The type of place, e.g. city, etc.
		/// </summary>
		[JsonProperty("place_type")]
		public string PlaceType { get; private set; }

		/// <summary>
		/// Url to metadata of this location.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; private set; }

		/// <summary>
		/// Returns the fullname of the place.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => FullName;
	}
}