using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Tag for a post on Bcy.
	/// </summary>
	public struct BcyPostTag
	{
		/// <summary>
		/// The id of the tag.
		/// </summary>
		[JsonProperty("tag_id")]
		public int TagId { get; private set; }
		/// <summary>
		/// The text value of the tag.
		/// </summary>
		[JsonProperty("tag_name")]
		public string TagName { get; private set; }
		/// <summary>
		/// The type of the object, e.g. tag or event.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The url to the image used for the tag.
		/// </summary>
		[JsonProperty("cover")]
		public Uri Cover { get; private set; }
		/// <summary>
		/// The id of the tag if this is an event.
		/// </summary>
		[JsonProperty("event_id")]
		public int? EventId { get; private set; }

		/// <summary>
		/// Returns the tag name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{TagName} ({TagId})";
	}
}