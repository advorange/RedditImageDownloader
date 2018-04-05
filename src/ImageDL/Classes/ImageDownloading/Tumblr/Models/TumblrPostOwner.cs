using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDL.Classes.ImageDownloading.Tumblr.Models
{
	public class Tumblelog2
	{
		[JsonProperty("title")]
		public string Title;
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("cname")]
		public bool Cname;
		[JsonProperty("url")]
		public string Url;
		[JsonProperty("timezone")]
		public string Timezone;
		[JsonProperty("avatar_url_16")]
		public string AvatarUrl16;
		[JsonProperty("avatar_url_24")]
		public string AvatarUrl24;
		[JsonProperty("avatar_url_30")]
		public string AvatarUrl30;
		[JsonProperty("avatar_url_40")]
		public string AvatarUrl40;
		[JsonProperty("avatar_url_48")]
		public string AvatarUrl48;
		[JsonProperty("avatar_url_64")]
		public string AvatarUrl64;
		[JsonProperty("avatar_url_96")]
		public string AvatarUrl96;
		[JsonProperty("avatar_url_128")]
		public string AvatarUrl128;
		[JsonProperty("avatar_url_512")]
		public string AvatarUrl512;
	}
}
