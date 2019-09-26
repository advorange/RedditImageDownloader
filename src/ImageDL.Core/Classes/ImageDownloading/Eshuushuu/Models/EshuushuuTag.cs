using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu.Models
{
	/// <summary>
	/// Holds the value and name of a tag.
	/// </summary>
	public struct EshuushuuTag
	{
		/// <summary>
		/// The name associated with the value.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <summary>
		/// The value to search with.
		/// </summary>
		[JsonProperty("value")]
		public int Value { get; private set; }

		/// <summary>
		/// Returns the name and value.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} ({Value})";
	}
}