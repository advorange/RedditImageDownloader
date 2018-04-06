using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu.Models
{
	/// <summary>
	/// Holds the value and name of a tag.
	/// </summary>
	public struct EshuushuuTag
	{
		/// <summary>
		/// The value to search with.
		/// </summary>
		[JsonProperty("value")]
		public readonly int Value;
		/// <summary>
		/// The name associated with the value.
		/// </summary>
		[JsonProperty("name")]
		public readonly string Name;

		/// <summary>
		/// Returns the name and value.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name} ({Value})";
		}
	}
}