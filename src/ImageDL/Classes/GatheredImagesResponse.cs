using System;
using System.Collections.Immutable;
using ImageDL.Enums;

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
		public readonly Uri Url;
		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUrls;

		/// <summary>
		/// Creates an instance of <see cref="GatheredImagesResponse"/>.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="reason"></param>
		/// <param name="error"></param>
		/// <param name="imageUrls"></param>
		private GatheredImagesResponse(Uri url, FailureReason reason, string error, params Uri[] imageUrls) : base(reason, error)
		{
			Url = url;
			ImageUrls = imageUrls.ToImmutableArray();
		}

		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri lead to an image.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromImage(Uri url)
		{
			return new GatheredImagesResponse(url, FailureReason.Success, null, url);
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the supplied uris.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="gathered"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromGatherer(Uri url, Uri[] gathered)
		{
			return new GatheredImagesResponse(url, FailureReason.Success, null, gathered);
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri lead to something animated.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromAnimated(Uri url)
		{
			return new GatheredImagesResponse(url, FailureReason.AnimatedContent, $"{url} is animated content.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that the uri didn't lead to any images.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromNotFound(Uri url)
		{
			return new GatheredImagesResponse(url, FailureReason.NotFound, $"{url} does not have any gatherable images.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error saying the exception.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromException(Uri url, Exception e)
		{
			return new GatheredImagesResponse(url, FailureReason.Exception, $"{url} had the exception: {e.Message}.");
		}
		/// <summary>
		/// Creates an <see cref="GatheredImagesResponse"/> with the error stating that it doesn't fit into any of the other sites.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static GatheredImagesResponse FromUnknown(Uri url)
		{
			return new GatheredImagesResponse(url, FailureReason.Misc, $"{url} is from an unknown website or otherwise unparsable.");
		}
	}
}