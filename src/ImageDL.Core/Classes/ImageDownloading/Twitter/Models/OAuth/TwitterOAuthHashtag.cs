using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// How to tag an image.
	/// </summary>
	public struct TwitterOAuthHashtag
	{
		/// <summary>
		/// Where the hashtag starts and ends in the text.
		/// </summary>
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		/// <summary>
		/// Name of the hashtag.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// Returns the text of the hashtag.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Text;
	}
}