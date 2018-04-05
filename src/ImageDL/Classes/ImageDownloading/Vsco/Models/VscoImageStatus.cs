#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Status of an image. Not sure what that means, I guess success/failure and at what time?
	/// </summary>
	public struct VscoImageStatus
	{
		[JsonProperty("code")]
		public readonly int Code;
		[JsonProperty("time")]
		private readonly long _Time;

		[JsonIgnore]
		public DateTime Time => (new DateTime(1970, 1, 1).AddSeconds(_Time / 1000)).ToUniversalTime();
	}
}
