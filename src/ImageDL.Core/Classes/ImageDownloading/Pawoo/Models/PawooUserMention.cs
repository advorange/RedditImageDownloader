using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// User mention for Pawoo.
	/// </summary>
	public struct PawooUserMention
	{
		/// <summary>
		/// The id of the mentioned user.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }
		/// <summary>
		/// Link to the user's account.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// Same as <see cref="Username"/>.
		/// </summary>
		[JsonProperty("acct")]
		public string Acct { get; private set; }

		/// <summary>
		/// Returns the username, and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Username} ({Id})";
		}
	}
}