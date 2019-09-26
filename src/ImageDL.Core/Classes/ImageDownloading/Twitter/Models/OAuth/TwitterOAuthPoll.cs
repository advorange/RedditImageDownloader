using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Poll on a tweet.
	/// </summary>
	public struct TwitterOAuthPoll
	{
		/// <summary>
		/// How long the poll goes for.
		/// </summary>
		[JsonProperty("duration_minutes")]
		public int DurationMinutes { get; private set; }

		/// <summary>
		/// When the poll ends in UTC.
		/// </summary>
		[JsonProperty("end_datetime")]
		public string EndDatetime { get; private set; }

		/// <summary>
		/// The options of the poll.
		/// </summary>
		[JsonProperty("options")]
		public IList<TwitterOAuthPollOption> Options { get; private set; }
	}
}