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

		public ContentLink(Uri uri, int score)
		{
			Uri = uri;
			Score = score;
		}

		public override string ToString()
		{
			return $"{Score} {Uri}";
		}
	}
}
