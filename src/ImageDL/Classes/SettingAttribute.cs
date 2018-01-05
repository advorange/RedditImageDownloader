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
		/// <summary>
		/// Indicates whether or not the setting has a default attribute and doesn't need to be set by the user.
		/// </summary>
		public readonly bool HasDefaultValue;

		public SettingAttribute(string description, bool hasDefaultValue = false)
		{
			Description = description;
			HasDefaultValue = hasDefaultValue;
		}
	}
}
