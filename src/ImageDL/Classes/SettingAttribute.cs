using System;

namespace ImageDL.Classes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingAttribute : Attribute
	{
		public readonly string Description;

		public SettingAttribute(string description)
		{
			Description = description;
		}
	}
}
