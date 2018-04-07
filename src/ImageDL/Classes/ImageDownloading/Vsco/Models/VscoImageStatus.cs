using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Status of an image. Not sure what that means, I guess success/failure and at what time?
	/// </summary>
	public struct VscoImageStatus
	{
		/// <summary>
		/// Status code. Presumably HTTP status code.
		/// </summary>
		[JsonProperty("code")]
		public int Code { get; private set; }
		/// <summary>
		/// The unix timestamp in milliseconds.
		/// </summary>
		[JsonProperty("time")]
		public long Timestamp { get; private set; }
		/// <summary>
		/// When the status was last changed.
		/// </summary>
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(Timestamp / 1000)).ToUniversalTime();
	}
}