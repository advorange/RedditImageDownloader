using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Methods that don't fit in any other class but are useful.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if two strings are the same.
		/// </summary>
		/// <param name="str1"></param>
		/// <param name="str2"></param>
		/// <returns></returns>
		public static bool CaseInsEquals(this string str1, string str2)
		{
			return String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if a string contains a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsContains(this string source, string search)
		{
			return source != null && search != null && source.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to return the index of a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public static bool CaseInsIndexOf(this string source, string search, out int position)
		{
			position = source == null || search == null ? -1 : source.IndexOf(search, StringComparison.OrdinalIgnoreCase);
			return position >= 0;
		}
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if a string ends with a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsStartsWith(this string source, string search)
		{
			return source != null && search != null && source.StartsWith(search, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if a string ends with a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsEndsWith(this string source, string search)
		{
			return source != null && search != null && source.EndsWith(search, StringComparison.OrdinalIgnoreCase);
		}
		/// <summary>
		/// Returns the string with the oldValue replaced with the newValue case insensitively.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static string CaseInsReplace(this string source, string oldValue, string newValue)
		{
			var sb = new StringBuilder();
			var previousIndex = 0;
			var index = source.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
			while (index != -1)
			{
				sb.Append(source.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = source.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
			}
			return sb.Append(source.Substring(previousIndex)).ToString();
		}
		/// <summary>
		/// Utilizes <see cref="CaseInsEquals(string, string)"/> to check if every string is the same.
		/// </summary>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static bool CaseInsEverythingSame(this IEnumerable<string> enumerable)
		{
			var array = enumerable.ToArray();
			for (int i = 1; i < array.Length; ++i)
			{
				if (!array[i - 1].CaseInsEquals(array[i]))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Utilizes <see cref="StringComparer.OrdinalIgnoreCase"/> to see if the search string is in the enumerable.
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsContains(this IEnumerable<string> enumerable, string search)
		{
			return enumerable.Contains(search, StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Splits <paramref name="input"/> similar to how command prompt splits arguments.
		/// </summary>
		/// <param name="input">The string to split.</param>
		/// <returns>An array of strings representing arguments.</returns>
		public static string[] SplitLikeCommandLine(this string input)
		{
			return input.Split('"').Select((x, index) =>
						{
							return index % 2 == 0
								? x.Split(new[] { ' ' })
								: new[] { x };
						}).SelectMany(x => x).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
		}
		/// <summary>
		/// Returns <see cref="DateTime.UtcNow"/> displaying years down to seconds.
		/// </summary>
		/// <returns>The time formatted neatly for saving files.</returns>
		public static string FormatDateTimeForSaving()
		{
			return DateTime.UtcNow.ToString("yyyyMMdd_hhmmss");
		}
		/// <summary>
		/// Adds in spaces between each capital letter.
		/// </summary>
		/// <param name="title">The title to format.</param>
		/// <returns>The passed in title with </returns>
		public static string FormatTitle(this string title)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < title.Length; ++i)
			{
				var c = title[i];
				if (Char.IsUpper(c) && (i > 0 && !Char.IsWhiteSpace(title[i - 1])))
				{
					sb.Append(' ');
				}
				sb.Append(c);
			}
			return sb.ToString();
		}
		/// <summary>
		/// Returns a hashed stream for file duplication checking.
		/// </summary>
		/// <param name="s">The stream to hash.</param>
		/// <returns>A string representing a hashed stream.</returns>
		public static string Hash<T>(this Stream s) where T : HashAlgorithm
		{
			s.Position = 0;
			string hash;
			using (var algorithm = (T)HashAlgorithm.Create(typeof(T).Name))
			{
				hash = BitConverter.ToString(algorithm.ComputeHash(s)).Replace("-", "").ToLower();
			}
			s.Position = 0;
			return hash;
		}
		/// <summary>
		/// Orders an <see cref="IEnumerable{T}"/> by something that does not implement <see cref="IComparable"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="input">The objects to order.</param>
		/// <param name="keySelector">The property to order by.</param>
		/// <returns>An ordered enumerable.</returns>
		public static IEnumerable<T> OrderByNonComparable<T, TKey>(this IEnumerable<T> input, Func<T, TKey> keySelector)
		{
			return input.GroupBy(keySelector).SelectMany(x => x);
		}
		/// <summary>
		/// Returns true if the passed in string is a valid url.
		/// </summary>
		/// <param name="input">The uri to evaluate.</param>
		/// <returns>A boolean indicating whether or not the string is a url.</returns>
		public static bool IsValidUrl(this string input)
		{
			return !String.IsNullOrWhiteSpace(input)
				&& Uri.TryCreate(input, UriKind.Absolute, out Uri uri)
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}
		/// <summary>
		/// Returns true if the passed in uri is a valid url.
		/// </summary>
		/// <param name="uri">The uri to evaluate.</param>
		/// <returns>A boolean indicating whether or not the uri is a url.</returns>
		public static bool IsValidUrl(this Uri uri)
		{
			return uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}
		/// <summary>
		/// Returns true for most image mime types (png, jpg, tiff, etc) but false for gif and anything else.
		/// </summary>
		/// <param name="path">The path or extension to check.</param>
		/// <returns>A boolean inicating whether or not the path leads to an image.</returns>
		public static bool IsImagePath(this string path)
		{
			var mimeType = MimeMapping.GetMimeMapping(path);
			return mimeType != "image/gif" && mimeType.CaseInsContains("image/");
		}
		/// <summary>
		/// Writes an exception to the console in <see cref="ConsoleColor.Red"/>.
		/// </summary>
		/// <param name="e">The passed in exception.</param>
		public static void Write(this Exception e)
		{
			var currColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ForegroundColor = currColor;
		}
		/// <summary>
		/// Creates a web request and sets some properties to make it look more human.
		/// </summary>
		/// <param name="uri">The site to navigate to.</param>
		/// <returns>A webrequest to <paramref name="uri"/>.</returns>
		public static HttpWebRequest CreateWebRequest(this Uri uri)
		{
			var req = (HttpWebRequest)WebRequest.Create(uri);
			req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
			req.Credentials = CredentialCache.DefaultCredentials;
			req.Timeout = 5000;
			req.ReadWriteTimeout = 5000;
			req.AllowAutoRedirect = true; //True so imgur can redirect to correct webpages
			req.CookieContainer = new CookieContainer();
			return req;
		}
	}
}
