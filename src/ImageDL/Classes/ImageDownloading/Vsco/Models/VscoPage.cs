#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for the results of a Vsco page.
	/// </summary>
	public class VscoPage
	{
		[JsonProperty("media")]
		public readonly List<VscoPost> Posts;
		[JsonProperty("page")]
		public readonly int PageNumber;
		[JsonProperty("size")]
		public readonly int Size;
		[JsonProperty("total")]
		public readonly int Total;
	}
}
