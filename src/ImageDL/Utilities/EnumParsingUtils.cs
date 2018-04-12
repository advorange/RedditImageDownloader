using System;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Utilities to help with parsing enums.
	/// </summary>
	public static class EnumParsingUtils
	{
		/// <summary>
		/// A <see cref="Classes.SettingParsing.TryParseDelegate{T}"/> for enums.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParseEnumCaseIns<T>(string s, out T result) where T : struct
		{
			return Enum.TryParse(s, true, out result);
		}
	}
}
