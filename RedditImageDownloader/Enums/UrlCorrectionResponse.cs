using System;

namespace MassDownloadImages.Enums
{
	/// <summary>
	/// The response from <see cref="HelperClasses.UriHelper.CorrectUri(Uri, out Uri)"/>.
	/// </summary>
	[Flags]
	public enum UriCorrectionResponse : uint
	{
		/// <summary>
		/// This uri is a valid image.
		/// </summary>
		Valid = (1U << 0),
		/// <summary>
		/// This uri is not a valid image or animated content.
		/// </summary>
		Invalid = (1U << 1),
		/// <summary>
		/// It is unknown whether or not this uri is valid.
		/// </summary>
		Unknown = (1U << 2),
		/// <summary>
		/// This uri is animated. Gif, mp4, etc.
		/// </summary>
		Animated = (1U << 3),
	}
}
