using System;

namespace ImageDL.Enums
{
	/// <summary>
	/// The site to target.
	/// </summary>
	[Flags]
	public enum Site : uint
	{
		/// <summary>
		/// Targetting the website https://www.reddit.com
		/// </summary>
		Reddit = (1U << 0),
	}
}
