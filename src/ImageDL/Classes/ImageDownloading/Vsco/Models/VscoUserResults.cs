#pragma warning disable 1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for the request of a Vsco user's information.
	/// </summary>
	public class VscoUserResults
	{
		[JsonProperty("sites")]
		public readonly List<VscoUserInfo> Users;
	}
}
