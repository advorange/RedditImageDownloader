#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Not sure what this holds.
	/// </summary>
	public struct InstagramRelatedMediaInfo
	{
		[JsonProperty("edges")]
		public readonly List<object> Nodes;
	}
}
