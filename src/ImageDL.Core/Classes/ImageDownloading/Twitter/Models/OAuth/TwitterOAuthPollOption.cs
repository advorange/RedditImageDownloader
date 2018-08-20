using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// An option in a poll.
	/// </summary>
	public struct TwitterOAuthPollOption
	{
		/// <summary>
		/// The position of the option. This is 1-based.
		/// </summary>
		[JsonProperty("position")]
		public int Position { get; private set; }
		/// <summary>
		/// The text of the option.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// Returns the position and the text.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Position} {Text}";
		}
	}
}