using System;
using System.Linq;

namespace RedditImageDownloader
{
	public static class HelperActions
	{
		public static bool CaseInsEquals(this string str1, string str2)
			=> String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		public static string[] SplitLikeCommandLine(this string input)
			=> input.Split('"').Select((x, index) =>
			{
				return index % 2 == 0
					? x.Split(new[] { ' ' })
					: new[] { x };
			}).SelectMany(x => x).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
		public static void WriteException(Exception e)
		{
			var currColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ForegroundColor = currColor;
		}
	}
}
