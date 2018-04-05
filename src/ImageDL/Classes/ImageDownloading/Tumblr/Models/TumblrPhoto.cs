using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	public class TumblrPhoto
	{
		[JsonProperty("offset")]
		public readonly string Offset;
		[JsonProperty("caption")]
		public readonly string Caption;
		[JsonProperty("width")]
		public readonly int Width;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("photo-url-1280")]
		public readonly string PhotoUrl1280;
		[JsonProperty("photo-url-500")]
		public readonly string PhotoUrl500;
		[JsonProperty("photo-url-400")]
		public readonly string PhotoUrl400;
		[JsonProperty("photo-url-250")]
		public readonly string PhotoUrl250;
		[JsonProperty("photo-url-100")]
		public readonly string PhotoUrl100;
		[JsonProperty("photo-url-75")]
		public readonly string PhotoUrl75;
	}
}
