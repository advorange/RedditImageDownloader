using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Information about how a post was created.
	/// </summary>
	public sealed class ArtstationMedium
	{
		/// <summary>
		/// The name of the medium.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The id of the medium.
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; private set; }

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
