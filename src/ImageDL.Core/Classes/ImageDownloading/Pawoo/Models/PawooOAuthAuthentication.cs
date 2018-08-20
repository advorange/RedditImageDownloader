using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// How the user was authenticated.
	/// </summary>
	public struct PawooOAuthAuthentication
	{
		/// <summary>
		/// Unique id.
		/// </summary>
		[JsonProperty("uid")]
		public string Uid { get; private set; }
		/// <summary>
		/// The name of the provider, e.g. pixiv, etc.
		/// </summary>
		[JsonProperty("provider")]
		public string Provider { get; private set; }

		/// <summary>
		/// Returns the provider and uid.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Provider} ({Uid})";
		}
	}
}