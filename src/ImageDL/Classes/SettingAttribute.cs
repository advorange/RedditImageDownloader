using System;

namespace ImageDL.Classes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingAttribute : Attribute
	{
		public readonly string Description;
		public readonly bool HasDefaultValue;

		public SettingAttribute(string description, bool hasDefaultValue = false)
		{
			Description = description;
			HasDefaultValue = hasDefaultValue;
		}
	}
}
