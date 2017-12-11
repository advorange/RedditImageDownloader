using System;

namespace MassDownloadImages.Classes
{
	/// <summary>
	/// The uri in here links to a gif, video, webm, etc.
	/// </summary>
	public struct AnimatedContent
	{
		/// <summary>
		/// The uri to the content.
		/// </summary>
		public Uri Uri;
		/// <summary>
		/// The score of the content. On reddit this is upvotes, on other sites it's other things.
		/// </summary>
		public int Score;

		public AnimatedContent(Uri uri, int score)
		{
			this.Uri = uri;
			this.Score = score;
		}
		
		public override string ToString() => $"{this.Score} {this.Uri}";
	}
}
