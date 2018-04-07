using System;
using System.Collections.Immutable;
using System.Linq;
using ImageDL.Enums;

namespace ImageDL.Classes
{
	/// <summary>
	/// Returns the images gathered with a reason if they haven't been gathered correctly.
	/// </summary>
	public class ImageResponse : Response
	{
		/// <summary>
		/// The images to download.
		/// </summary>
		public readonly ImmutableArray<Uri> ImageUrls;

		/// <summary>
		/// Creates an instance of <see cref="ImageResponse"/>.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="error"></param>
		/// <param name="imageUrls"></param>
		public ImageResponse(FailureReason reason, string error, params Uri[] imageUrls) : base(reason, error)
		{
			ImageUrls = (imageUrls ?? Enumerable.Empty<Uri>()).ToImmutableArray();
		}
	}
}