using MassDownloadImages.Enums;
using System;
using System.IO;
using System.Linq;

namespace MassDownloadImages.HelperClasses
{
	/// <summary>
	/// Methods which are used for uris.
	/// </summary>
	public static class UriHelper
	{
		private static string[] _AnimatedExtensions = new[]
		{
			".gif", ".gifv",
			".mp4",
			".webm",
		};
		private static string[] _AnimatedSites = new[]
		{
			"youtu.be", "youtube",
			"gfycat",
			"streamable",
		};
		private static string[] _InvalidExtensions = new[]
		{
			".html",
		};
		private static string[] _InvalidSites = new[]
		{
			"twitter",
		};

		/// <summary>
		/// Returns an array of <see cref="Uri"/> which link to images.
		/// </summary>
		/// <param name="uri">The uri to either use as an image or gather images from.</param>
		/// <returns>An array of <see cref="Uri"/> to images.</returns>
		public static Uri[] GetImageUris(Uri uri)
		{
			switch (uri.ToString())
			{
				#region Imgur
				case string u when u.Contains("imgur") && (u.Contains("/a/") || u.Contains("/gallery/")):
				{
					return ImgurScraping.ScrapeImages(uri);
				}
				#endregion
				default:
				{
					return new[] { uri };
				}
				//TODO: specify for tumblr and reddit image hosting
			}
		}
		/// <summary>
		/// Decides whether or not a <see cref="Uri"/> is a valid image.
		/// </summary>
		/// <param name="uri">The passed in <see cref="Uri"/> to check.</param>
		/// <param name="correctedUri">The corrected <see cref="Uri"/>.</param>
		/// <returns>A <see cref="UriCorrectionResponse"/> indicating the status of <paramref name="uri"/>.</returns>
		public static UriCorrectionResponse CorrectUri(Uri uri, out Uri correctedUri)
		{
			switch (uri.ToString())
			{
				//If it already has an extention then use that
				case string u when Path.GetExtension(u) is string ext && !String.IsNullOrWhiteSpace(ext):
				{
					if (_AnimatedExtensions.Contains(ext))
					{
						correctedUri = new Uri(u);
						return UriCorrectionResponse.Animated;
					}
					else if (_InvalidExtensions.Contains(ext))
					{
						correctedUri = null;
						return UriCorrectionResponse.Invalid;
					}
					else
					{
						correctedUri = new Uri(u);
						return UriCorrectionResponse.Valid;
					}
				}
				case string u when _AnimatedSites.Any(x => u.Contains(x)):
				{
					correctedUri = new Uri(u);
					return UriCorrectionResponse.Animated;
				}
				case string u when _InvalidSites.Any(x => u.Contains(x)):
				{
					correctedUri = null;
					return UriCorrectionResponse.Invalid;
				}
				case string u when u.Contains("imgur") && u.Contains("_d") && u.Contains("maxwidth"):
				{
					//I don't know what the purpose of maxwidth is, but when it's in the uri it always adds _d to the image id
					//and if that's left in it makes the image really small, so that's why both get removed.
					correctedUri = new Uri(u.Substring(0, u.IndexOf("?")).Replace("_d", ""));
					return UriCorrectionResponse.Valid;
				}
				default:
				{
					correctedUri = new Uri(uri.ToString() + ".png");
					return UriCorrectionResponse.Unknown;
				}
			}
		}
	}
}
