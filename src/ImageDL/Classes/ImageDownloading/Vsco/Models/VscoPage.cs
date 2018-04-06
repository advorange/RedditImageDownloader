using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for the results of a Vsco page.
	/// </summary>
	public class VscoPage
	{
		/// <summary>
		/// The posts on the page.
		/// </summary>
		[JsonProperty("media")]
		public readonly List<VscoPost> Posts;
		/// <summary>
		/// The current page number.
		/// </summary>
		[JsonProperty("page")]
		public readonly int PageNumber;
		/// <summary>
		/// How many posts were found on this page.
		/// </summary>
		[JsonProperty("size")]
		public readonly int Size;
		/// <summary>
		/// The sum of the amount of posts found on every page before and including this one.
		/// </summary>
		[JsonProperty("total")]
		public readonly int Total;
	}
}