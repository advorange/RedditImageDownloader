using System;

namespace ImageDL.Core.Utilities
{
	/// <summary>
	/// Utilities for ImageDL.
	/// </summary>
	public static class ImageDLUtils
	{
		/// <summary>
		/// Parses an enum case insensitively.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="s"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryParseCaseIns<T>(string s, out T value) where T : struct, Enum
			=> Enum.TryParse(s, true, out value);
	}
}
