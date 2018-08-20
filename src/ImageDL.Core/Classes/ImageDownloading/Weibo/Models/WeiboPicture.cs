using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about the regular and large size of a Weibo image.
	/// </summary>
	public struct WeiboPicture
	{
		/// <summary>
		/// Unique id for the image.
		/// </summary>
		[JsonProperty("pid")]
		public string Pid { get; private set; }
		/// <summary>
		/// Url to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The type of image, if this is not the large one this will usually be 'orj360' indicating it was scaled down.
		/// </summary>
		[JsonProperty("size")]
		public string Size { get; private set; }
		/// <summary>
		/// The size of the image.
		/// </summary>
		[JsonProperty("geo")]
		public WeiboGeo Geo { get; private set; }
		/// <summary>
		/// The full size version of the image.
		/// </summary>
		[JsonProperty("large")]
		public WeiboLargePicture Large { get; private set; }

		/// <summary>
		/// Returns the string value of the large image.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Large.ToString();
		}
	}
}