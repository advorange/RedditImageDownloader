using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information about a category on Artstation.
	/// </summary>
	public struct ArtstationCategory
	{
		/// <summary>
		/// The value of the category.
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; private set; }

		/// <summary>
		/// The name of the category.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// Returns the name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} ({Id})";
	}
}