using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// The id of the pin in the current language.
	/// </summary>
	public struct PinterestCanonicalPin
	{
		/// <summary>
		/// The id of the pin.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}