using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// Indicates the property is a setting that can be set to affect functionality of the downloader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingAttribute : Attribute
	{
		/// <summary>
		/// Short text describing what the field does.
		/// </summary>
		public readonly string Description;

		public SettingAttribute(string description)
		{
			Description = description;
		}
	}
}
