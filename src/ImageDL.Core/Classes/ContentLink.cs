using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// Holds the location of the content and a number assocated to it (can be score, id, etc).
	/// </summary>
	public struct ContentLink
	{
		/// <summary>
		/// The uri to the content.
		/// </summary>
		public readonly Uri Uri;
		/// <summary>
		/// The number to associate with this content. Can be score, id, etc.
		/// </summary>
		public readonly int AssociatedNumber;
		/// <summary>
		/// The reason why this wasn't downloaded. E.G. failed download, not a static image.
		/// </summary>
		public readonly string Reason;
		
		/// <summary>
		/// Creates a content link, which stores a uri, number (score, etc), and the reason for creating it.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="associatedNumber"></param>
		/// <param name="reason"></param>
		public ContentLink(Uri uri, int associatedNumber, string reason)
		{
			Uri = uri;
			AssociatedNumber = associatedNumber;
			Reason = reason;
		}

		/// <summary>
		/// Returns the number and the uri.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{AssociatedNumber} {Uri}";
		}
	}
}
