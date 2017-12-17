using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			=> String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if a string contains a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsContains(this string source, string search)
			=> source != null && search != null && source.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
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
			=> source != null && search != null && source.StartsWith(search, StringComparison.OrdinalIgnoreCase);
		/// <summary>
		/// Utilizes <see cref="StringComparison.OrdinalIgnoreCase"/> to check if a string ends with a search string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		public static bool CaseInsEndsWith(this string source, string search)
			=> source != null && search != null && source.EndsWith(search, StringComparison.OrdinalIgnoreCase);
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
			=> enumerable.Contains(search, StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Splits <paramref name="input"/> similar to how command prompt splits arguments.
		/// </summary>
		/// <param name="input">The string to split.</param>
		/// <returns>An array of strings representing arguments.</returns>
		public static string[] SplitLikeCommandLine(this string input)
			=> input.Split('"').Select((x, index) =>
			{
				return index % 2 == 0
					? x.Split(new[] { ' ' })
					: new[] { x };
			}).SelectMany(x => x).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
		/// <summary>
		/// Returns <see cref="DateTime.UtcNow"/> displaying years down to seconds.
		/// </summary>
		/// <returns>The time formatted neatly for saving files.</returns>
		public static string FormatDateTimeForSaving()
			=> DateTime.UtcNow.ToString("yyyyMMdd_hhmmss");
		/// <summary>
		/// Returns the amount of characters in a number.
		/// </summary>
		/// <param name="num">The number to get the length of.</param>
		/// <returns>The amount of characters in a number.</returns>
		public static int GetLengthOfNumber(this int num)
			=> num.ToString().Length;
		/// <summary>
		/// Orders an <see cref="IEnumerable{T}"/> by something that does not implement <see cref="IComparable"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="input">The objects to order.</param>
		/// <param name="keySelector">The property to order by.</param>
		/// <returns>An ordered enumerable.</returns>
		public static IEnumerable<T> OrderByNonComparable<T, TKey>(this IEnumerable<T> input, Func<T, TKey> keySelector)
			=> input.GroupBy(keySelector).SelectMany(x => x);

		/// <summary>
		/// Writes an exception to the console in <see cref="ConsoleColor.Red"/>.
		/// </summary>
		/// <param name="e">The passed in exception.</param>
		public static void WriteException(this Exception e)
		{
			var currColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ForegroundColor = currColor;
		}
	}
}
