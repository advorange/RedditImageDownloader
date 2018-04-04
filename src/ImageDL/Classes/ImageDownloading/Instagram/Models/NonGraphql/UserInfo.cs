#pragma warning disable 1591
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.NonGraphql
{
	/// <summary>
	/// Holds information relating to the user.
	/// </summary>
	public sealed class UserInfo
	{
		[JsonProperty("edge_owner_to_timeline_media")]
		public readonly MediaTimeline Content;
	}
}
