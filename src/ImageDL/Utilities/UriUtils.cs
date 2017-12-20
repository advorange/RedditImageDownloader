using ImageDL.Enums;
using ImageDL.Utilities.Scraping;
using System;
using System.IO;
using System.Linq;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Methods which are used for uris.
	/// </summary>
	public static class UriUtils
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

		/// <summary>
		/// Returns an array of <see cref="Uri"/> which link to images.
		/// </summary>
		/// <param name="uri">The uri to either use as an image or gather images from.</param>
		/// <returns>An array of <see cref="Uri"/> to images.</returns>
		public static Uri[] GetImageUris(Uri uri)
		{
			switch (uri.ToString())
			{
				case string u when u.Contains("imgur") && (u.Contains("/a/") || u.Contains("/gallery/")):
				{
					return ImgurScraping.ScrapeImages(uri);
				}
				case string u when u.Contains("deviantart") && u.Contains("/art/"):
				{
					return DeviantArtScraping.ScrapeImages(uri);
				}
				case string u when u.Contains("instagram") && u.Contains("/p/"):
				{
					return InstagramScraping.ScrapeImages(uri);
				}
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
			var str = uri.ToString();
			//Anything after the question mark isn't necessary because it's optional arguments used by the website
			if (str.CaseInsContains("?"))
			{
				str = str.Substring(0, str.IndexOf("?"));
			}

			switch (str)
			{
				//If it already has an extention then use that
				case string u when Path.GetExtension(u) is string ext && !String.IsNullOrWhiteSpace(ext):
				{
					if (_AnimatedExtensions.CaseInsContains(ext))
					{
						correctedUri = new Uri(u);
						return UriCorrectionResponse.Animated;
					}
					else if (_InvalidExtensions.CaseInsContains(ext))
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
				case string u when _AnimatedSites.Any(x => u.CaseInsContains(x)):
				{
					correctedUri = new Uri(u);
					return UriCorrectionResponse.Animated;
				}
				case string u when u.CaseInsContains("imgur.com"):
				{
					return CorrectImgurUri(uri, out correctedUri);
				}
				case string u when u.CaseInsContains("reddit.com") || u.CaseInsContains("redd.it"):
				{
					return CorrectRedditUri(uri, out correctedUri);
				}
				case string u when u.CaseInsContains("twitter.com"):
				{
					return CorrectTwitterUri(uri, out correctedUri);
				}
				case string u when u.CaseInsContains("instagram.com"):
				{
					return CorrectInstagramUri(uri, out correctedUri);
				}
				case string u when u.CaseInsContains("deviantart.com"):
				{
					return CorrectDeviantArtUri(uri, out correctedUri);
				}
				default:
				{
					correctedUri = uri;
					return UriCorrectionResponse.Unknown;
				}
			}
		}
		/// <summary>
		/// Decides whether or not an Imgur <see cref="Uri"/> is a valid image.
		/// </summary>
		/// <param name="uri">The passed in Imgur <see cref="Uri"/> to check.</param>
		/// <param name="correctedUri">The corrected <see cref="Uri"/>.</param>
		/// <returns>A <see cref="UriCorrectionResponse"/> indicating the status of <paramref name="uri"/>.</returns>
		public static UriCorrectionResponse CorrectImgurUri(Uri uri, out Uri correctedUri)
		{
			var u = uri.ToString();

			//Example: https://i.imgur.com/PxAqScg_d.jpg
			//_d seems to make the image a thumbnail?
			if (u.CaseInsContains("_d."))
			{
				u = u.Replace("_d.", ".");
			}
			if (String.IsNullOrWhiteSpace(Path.GetExtension(u)))
			{
				u += ".png";
			}

			correctedUri = new Uri(u);
			return UriCorrectionResponse.Valid;
		}
		/// <summary>
		/// Decides whether or not a reddit <see cref="Uri"/> is a valid image.
		/// </summary>
		/// <param name="uri">The passed in reddit <see cref="Uri"/> to check.</param>
		/// <param name="correctedUri">The corrected <see cref="Uri"/>.</param>
		/// <returns>A <see cref="UriCorrectionResponse"/> indicating the status of <paramref name="uri"/>.</returns>
		public static UriCorrectionResponse CorrectRedditUri(Uri uri, out Uri correctedUri)
		{
			var u = uri.ToString();

			if (u.CaseInsContains("v.redd.it"))
			{
				correctedUri = new Uri(u);
				return UriCorrectionResponse.Animated;
			}

			correctedUri = new Uri(u);
			return UriCorrectionResponse.Valid;
		}
		//TODO
		public static UriCorrectionResponse CorrectTwitterUri(Uri uri, out Uri correctedUri)
		{
			correctedUri = null;
			return UriCorrectionResponse.Invalid;
		}
		public static UriCorrectionResponse CorrectInstagramUri(Uri uri, out Uri correctedUri)
		{
			correctedUri = null;
			return UriCorrectionResponse.Invalid;
		}
		public static UriCorrectionResponse CorrectDeviantArtUri(Uri uri, out Uri correctedUri)
		{
			correctedUri = null;
			return UriCorrectionResponse.Invalid;
		}
		/// <summary>
		/// Returns true if the passed in string is a valid url.
		/// </summary>
		/// <param name="input">The uri to evaluate.</param>
		/// <returns>A boolean indicating whether or not the string is a url.</returns>
		public static bool GetIfStringIsValidUrl(string input) => true
			&& !String.IsNullOrWhiteSpace(input)
			&& Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult)
			&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		/// <summary>
		/// Returns true if the passed in uri is a valid url.
		/// </summary>
		/// <param name="uri">The uri to evaluate.</param>
		/// <returns>A boolean indicating whether or not the uri is a url.</returns>
		public static bool GetIfUriIsValidUrl(Uri uri) => true
			&& uri.IsAbsoluteUri
			&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
	}
}
