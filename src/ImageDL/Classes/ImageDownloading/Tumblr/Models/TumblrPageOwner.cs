using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	public class TumblrPageOwner
	{
		[JsonProperty("title")]
		public readonly string Title;
		[JsonProperty("description")]
		public readonly string Description;
		[JsonProperty("name")]
		public readonly string Name;
		[JsonProperty("timezone")]
		public readonly string Timezone;
		[JsonProperty("cname")]
		public readonly bool Cname;
		[JsonProperty("feeds")]
		public readonly List<object> Feeds;
	}
}
