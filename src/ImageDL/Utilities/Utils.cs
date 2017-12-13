using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Methods that don't fit in any other class but are useful.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Checks if two strings are equal case insensitively.
		/// </summary>
		/// <param name="str1">First string.</param>
		/// <param name="str2">Second string.</param>
		/// <returns>Returns true if both strings are equal.</returns>
		public static bool CaseInsEquals(this string str1, string str2)
			=> String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
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
			=> num == 0 ? 1 : (int)Math.Log10(Math.Abs(num)) + 1;
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
