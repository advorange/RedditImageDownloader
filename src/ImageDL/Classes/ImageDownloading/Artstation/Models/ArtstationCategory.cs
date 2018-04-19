using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information about a category on Artstation.
	/// </summary>
	public struct ArtstationCategory
	{
		/// <summary>
		/// The name of the category.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The value of the category.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
	}
}
