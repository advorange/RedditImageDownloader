using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Location information of a post.
	/// </summary>
	public struct TwitterOAuthCoordinates
	{
		/// <summary>
		/// Longitude, then latitude.
		/// </summary>
		[JsonProperty("coordinates")]
		public IList<float> Coordinates { get; private set; }
		/// <summary>
		/// The type of coordinates, e.g. point, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }

		/// <summary>
		/// Returns the latitude and longitude in non degree format.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Coordinates[0]}x{Coordinates[1]}";
	}
}