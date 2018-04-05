using ImageDL.Enums;
using System;
using System.Collections.Immutable;

namespace ImageDL.Classes
{
	/// <summary>
	/// Returns the images gathered with a reason if they haven't been gathered correctly.
	/// </summary>
	public class GatheredImagesResponse : Response
	{
		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUris;

		/// <summary>
		/// Creates an instance of <see cref="GatheredImagesResponse"/>.
		/// </summary>
		/// <param name="originalUri"></param>
		/// <param name="reason"></param>
		/// <param name="error"></param>
		/// <param name="gatheredUris"></param>
		private GatheredImagesResponse(Uri originalUri, FailureReason reason, string error, params Uri[] gatheredUris) : base(reason, error)
		{
			OriginalUri = originalUri;
			ImageUris = gatheredUris.ToImmutableArray();
		}

		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri lead to an image.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromImage(Uri uri)
		{
			return new GatheredImagesResponse(uri, FailureReason.Success, null, uri);
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the supplied uris.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="gathered"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromGatherer(Uri uri, Uri[] gathered)
		{
			return new GatheredImagesResponse(uri, FailureReason.Success, null, gathered);
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri lead to something animated.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromAnimated(Uri uri)
		{
			return new GatheredImagesResponse(uri, FailureReason.AnimatedContent, $"{uri} is animated content.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri didn't lead to any images.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromNotFound(Uri uri)
		{
			return new GatheredImagesResponse(uri, FailureReason.NotFound, $"{uri} does not have any gatherable images.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error saying the exception.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromException(Uri uri, Exception e)
		{
			return new GatheredImagesResponse(uri, FailureReason.Exception, $"{uri} had the exception: {e.Message}.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that it doesn't fit into any of the other sites.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromUnknown(Uri uri)
		{
			return new GatheredImagesResponse(uri, FailureReason.Misc, $"{uri} is from an unknown website or otherwise unparsable.");
		}
	}
}