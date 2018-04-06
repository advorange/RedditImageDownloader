using System;
using ImageDL.Enums;

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
		public readonly Uri Url;
		/// <summary>
		/// The number to associate with this content. Can be score, id, etc.
		/// </summary>
		public readonly int AssociatedNumber;
		/// <summary>
		/// The reason why this wasn't downloaded. E.G. failed download, not a static image.
		/// </summary>
		public readonly FailureReason Reason;
		
		/// <summary>
		/// Creates a content link, which stores a url, number (score, etc), and the reason for creating it.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="associatedNumber"></param>
		/// <param name="reason"></param>
		public ContentLink(Uri url, int associatedNumber, FailureReason reason)
		{
			Url = url;
			AssociatedNumber = associatedNumber;
			Reason = reason;
		}

		/// <summary>
		/// Returns the number and the uri.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{AssociatedNumber} {Url}";
		}
	}
}