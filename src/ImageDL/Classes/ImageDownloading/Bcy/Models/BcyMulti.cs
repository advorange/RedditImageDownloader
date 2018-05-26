using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Effectively acts as a thumbnail.
	/// </summary>
	public struct BcyMulti
	{
		/// <summary>
		/// Url to more images with the source.
		/// </summary>
		[JsonProperty("path")]
		public string Path { get; private set; }

		/// <summary>
		/// Returns the path.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Path;
		}
	}
}