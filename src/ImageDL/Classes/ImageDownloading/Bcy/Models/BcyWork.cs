using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Information about the source of characters.
	/// </summary>
	public struct BcyWork
	{
		/// <summary>
		/// The name of the source.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
		/// <summary>
		/// Url to more images with the source.
		/// </summary>
		[JsonProperty("link")]
		public string Link { get; private set; }

		/// <summary>
		/// Returns the text.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Text;
		}
	}
}