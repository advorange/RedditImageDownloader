using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for the request of a Vsco user's information.
	/// </summary>
	public class VscoUserResults
	{
		/// <summary>
		/// The gathered users.
		/// </summary>
		[JsonProperty("sites")]
		public readonly List<VscoUserInfo> Users;
	}
}