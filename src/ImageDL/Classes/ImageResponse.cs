using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using AdvorangesUtils;
using ImageDL.Enums;

namespace ImageDL.Classes
{
	/// <summary>
	/// Returns the images gathered with a reason if they haven't been gathered correctly.
	/// </summary>
	public class ImageResponse : Response
	{
		/// <summary>
		/// Indicates the url lead to an image or multiple images.
		/// </summary>
		public const string SUCCESS = "Success";
		/// <summary>
		/// Indicates the url leads to animated content.
		/// </summary>
		public const string ANIMATED = "Animated";
		/// <summary>
		/// Indicates where the url leads to is unknown.
		/// </summary>
		public const string UNKNOWN = "Unknown";
		/// <summary>
		/// Indicates no images could be found at the url.
		/// </summary>
		public const string NOT_FOUND = "Not Found";
		/// <summary>
		/// Indicates the url has already been downloaded.
		/// </summary>
		public const string ALREADY_DOWNLOADED = "Already Downloaded";
		/// <summary>
		/// Indicates the url had an exception during downloading.
		/// </summary>
		public const string EXCEPTION = "Exception";

		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUrls;

		/// <summary>
		/// Creates an instance of <see cref="ImageResponse"/>.
		/// </summary>
		/// <param name="reasonType"></param>
		/// <param name="text"></param>
		/// <param name="isSuccess"></param>
		/// <param name="urls"></param>
		private ImageResponse(string reasonType, string text, bool? isSuccess, IEnumerable<Uri> urls)
			: base(reasonType, text, isSuccess)
		{
			ImageUrls = (urls ?? Enumerable.Empty<Uri>()).ToImmutableArray();
		}

		/// <summary>
		/// Returns a response indicating success with the passed in urls.
		/// </summary>
		/// <param name="urls"></param>
		/// <returns></returns>
		public static ImageResponse FromImages(IEnumerable<Uri> urls)
		{
			return new ImageResponse(SUCCESS, $"{urls.Count()} valid image(s) found.", true, urls);
		}
		/// <summary>
		/// Returns a response indicating failure with the passed in url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static ImageResponse FromAnimated(Uri url)
		{
			return new ImageResponse(ANIMATED, $"{url} is either a video or a gif.", false, new[] { url });
		}
		/// <summary>
		/// Returns a response of success if the url leads to an image, otherwise returns unknown.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static ImageResponse FromUrl(Uri url)
		{
			if (url.ToString().IsImagePath())
			{
				return FromImages(new[] { url });
			}
			return new ImageResponse(UNKNOWN, $"{url} cannot have its content type determined.", null, new[] { url });
		}
		/// <summary>
		/// Returns a response indicating failure with the passed in url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static ImageResponse FromNotFound(Uri url)
		{
			return new ImageResponse(NOT_FOUND, $"{url} did not have any images.", false, new[] { url });
		}
		/// <summary>
		/// Returns a response indicating failure.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static ImageResponse FromException(Uri url, Exception e)
		{
			return new ImageResponse(EXCEPTION, $"{url} had the following exception:\n{e}", false, new[] { url });
		}
	}
}