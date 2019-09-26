using System;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about a video.
	/// </summary>
	public struct WeiboMediaInfo
	{
		/// <summary>
		/// The url to the video.
		/// </summary>
		[JsonProperty("stream_url")]
		public Uri StreamUrl { get; private set; }

		/// <summary>
		/// Returns the stream url.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => StreamUrl.ToString();
	}
}