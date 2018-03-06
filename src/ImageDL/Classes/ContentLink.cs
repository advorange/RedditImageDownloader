using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// Holds the location and score of content.
	/// </summary>
	public struct ContentLink
	{
		/// <summary>
		/// The uri to the content.
		/// </summary>
		public readonly Uri Uri;
		/// <summary>
		/// The score of the content.
		/// </summary>
		public readonly int Score;
		/// <summary>
		/// The reason why this wasn't downloaded. E.G. failed download, not a static image.
		/// </summary>
		public readonly string Reason;

		public ContentLink(Uri uri, int score, string reason)
		{
			Uri = uri;
			Score = score;
			Reason = reason;
		}

		public override string ToString()
		{
			return $"{Score} {Uri}";
		}
	}
}
