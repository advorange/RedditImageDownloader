using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Current results and the status of an Instagram query.
	/// </summary>
	public sealed class InstagramResult
	{
		/// <summary>
		/// All the information from a user's page.
		/// </summary>
		[JsonProperty("data")]
		public InstagramData Data { get; private set; }

		/// <summary>
		/// HTTP status response.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; private set; }
	}
}