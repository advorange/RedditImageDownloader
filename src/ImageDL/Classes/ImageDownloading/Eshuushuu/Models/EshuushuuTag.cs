#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu.Models
{
	/// <summary>
	/// Holds the value and name of a tag.
	/// </summary>
	public struct EshuushuuTag
	{
		[JsonProperty("value")]
		public readonly int Value;
		[JsonProperty("name")]
		public readonly string Name;
	}
}
