#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information.
	/// </summary>
	public sealed class Data
	{
		[JsonProperty("user")]
		public readonly UserInfo User;
	}
}
