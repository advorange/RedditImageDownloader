using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// The uri in here links to a gif, video, webm, etc.
	/// </summary>
	public struct AnimatedContent
	{
		/// <summary>
		/// The uri to the content.
		/// </summary>
		public readonly Uri Uri;
		/// <summary>
		/// The score of the content.
		/// </summary>
		public readonly int Score;

		public AnimatedContent(Uri uri, int score)
		{
			Uri = uri;
			Score = score;
		}
		
		public override string ToString() => $"{Score} {Uri}";
	}
}
