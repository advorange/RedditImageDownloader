using System;

namespace ImageDL.Classes
{
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

		public override string ToString() => $"{Score} {Uri}";
	}
}
