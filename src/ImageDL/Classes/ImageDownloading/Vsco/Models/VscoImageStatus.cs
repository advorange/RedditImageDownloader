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
		public readonly int Code;
		/// <summary>
		/// The unix timestamp in milliseconds.
		/// </summary>
		[JsonProperty("time")]
		public readonly long Timestamp;

		/// <summary>
		/// When the status was last changed.
		/// </summary>
		public DateTime Time => (new DateTime(1970, 1, 1).AddSeconds(Timestamp / 1000)).ToUniversalTime();
	}
}