using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Information about a tag.
	/// </summary>
	public struct PawooTag
	{
		/// <summary>
		/// The name of the tag.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The url leading to the tag.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
	}
}