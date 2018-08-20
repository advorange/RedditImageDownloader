using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about the large size of a Weibo image.
	/// </summary>
	public struct WeiboLargePicture
	{
		/// <summary>
		/// The type of image, if this is the large one this will usually be 'large' indicating it is full size.
		/// </summary>
		[JsonProperty("size")]
		public string Size { get; private set; }
		/// <summary>
		/// Url to the image.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// The size of the image.
		/// </summary>
		[JsonProperty("geo")]
		public WeiboGeo Geo { get; private set; }

		/// <summary>
		/// Returns the url and resolution.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Url} {Geo}";
		}
	}
}