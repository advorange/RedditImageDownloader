using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Encloses a space.
	/// </summary>
	public struct TwitterOAuthBoundingBox
	{
		/// <summary>
		/// The points which outline the location.
		/// </summary>
		[JsonProperty("coordinates")]
		public IList<IList<IList<float>>> Coordinates { get; private set; }
		/// <summary>
		/// The type of coordinates, e.g. polygon, etc.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
	}
}