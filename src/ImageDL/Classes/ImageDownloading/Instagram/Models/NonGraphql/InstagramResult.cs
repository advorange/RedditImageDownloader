#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Current results and the status of an Instagram query.
	/// </summary>
	public sealed class InstagramResult
	{
		[JsonProperty("data")]
		public readonly Data Data;
		[JsonProperty("status")]
		public readonly string Status;
	}
}
