using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Article metadata for a post.
	/// </summary>
	public class PinterestArticle
	{
		/// <summary>
		/// The article's description.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// When it was posted, if possibly gotten.
		/// </summary>
		[JsonProperty("date_published")]
		public string DatePublished { get; private set; }
		/// <summary>
		/// The people who created this.
		/// </summary>
		[JsonProperty("authors")]
		public IList<object> Authors { get; private set; }
		/// <summary>
		/// The type of object this is, e.g. articlemetadata.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The id of the article.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		/// The name of whoever created this.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// Returns the name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name} ({Id})";
		}
	}
}