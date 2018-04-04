using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImageDL.Classes
{
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public struct ImagesResult
	{
		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// Indicates whether or not the link leads to a video site.
		/// </summary>
		public readonly bool IsAnimated;
		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUris;
		/// <summary>
		/// Any errors which have occurred during getting <see cref="ImageUris"/>.
		/// </summary>
		public readonly string Error;

		/// <summary>
		/// Creates an instance of <see cref="ImagesResult"/>.
		/// </summary>
		/// <param name="originalUri"></param>
		/// <param name="isAnimated"></param>
		/// <param name="gatheredUris"></param>
		/// <param name="error"></param>
		private ImagesResult(Uri originalUri, bool isAnimated, IEnumerable<Uri> gatheredUris, string error)
		{
			OriginalUri = originalUri;
			IsAnimated = isAnimated;
			ImageUris = gatheredUris.ToImmutableArray();
			Error = error;
		}

		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the error stating that the uri lead to an image.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static ImagesResult FromImage(Uri uri)
		{
			return new ImagesResult(uri, false, new[] { uri }, null);
		}
		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the supplied uris.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="gathered"></param>
		/// <returns></returns>
		public static ImagesResult FromGatherer(Uri uri, IEnumerable<Uri> gathered)
		{
			return new ImagesResult(uri, false, gathered, null);
		}
		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the error stating that the uri lead to something animated.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static ImagesResult FromAnimated(Uri uri)
		{
			return new ImagesResult(uri, true, new Uri[0], $"{uri} is animated content (gif/video).");
		}
		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the error stating that the uri didn't lead to any images.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static ImagesResult FromNotFound(Uri uri)
		{
			return new ImagesResult(uri, false, new Uri[0], $"{uri} doesn't link to any images.");
		}
		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the error saying the exception.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static ImagesResult FromException(Uri uri, Exception e)
		{
			return new ImagesResult(uri, false, new Uri[0], e.Message);
		}
		/// <summary>
		/// Creates an <see cref="ImagesResult"/> with the error stating that it doesn't fit into any of the other sites.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static ImagesResult FromMisc(Uri uri)
		{
			return new ImagesResult(uri, false, new Uri[0], $"{uri} is from an unknown website or is unable to be parsed.");
		}
	}
}