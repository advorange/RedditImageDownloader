using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// User mentions in the post.
	/// </summary>
	public struct TwitterOAuthUserMention
	{
		/// <summary>
		/// Where the user mention starts and ends in the text.
		/// </summary>
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		/// <summary>
		/// The id of the mentioned user.
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; private set; }
		/// <summary>
		/// String of <see cref="Id"/>.
		/// </summary>
		[JsonProperty("id_str")]
		public string IdStr { get; private set; }
		/// <summary>
		/// The screen name of the user.
		/// </summary>
		[JsonProperty("screen_name")]
		public string ScreenName { get; private set; }
		/// <summary>
		/// The display name of the user.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
	}
}