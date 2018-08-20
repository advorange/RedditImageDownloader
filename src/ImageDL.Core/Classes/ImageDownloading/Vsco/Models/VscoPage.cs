using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for the results of a Vsco page.
	/// </summary>
	public struct VscoPage
	{
		/// <summary>
		/// The posts on the page.
		/// </summary>
		[JsonProperty("media")]
		public IList<VscoPost> Posts { get; private set; }
		/// <summary>
		/// The current page number.
		/// </summary>
		[JsonProperty("page")]
		public int PageNumber { get; private set; }
		/// <summary>
		/// How many posts were found on this page.
		/// </summary>
		[JsonProperty("size")]
		public int Size { get; private set; }
		/// <summary>
		/// The sum of the amount of posts found on every page before and including this one.
		/// </summary>
		[JsonProperty("total")]
		public int Total { get; private set; }
	}
}