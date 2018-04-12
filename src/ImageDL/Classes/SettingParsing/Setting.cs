using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdvorangesUtils;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// An abstract class for options.
	/// </summary>
	public abstract class Setting
	{
		/// <summary>
		/// String indicating what this setting does.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// String with information about the setting.
		/// </summary>
		public abstract string Information { get; }
		/// <summary>
		/// Indicates the setting is a boolean which only requires an attempt at parsing it for it to set itself to true.
		/// The passed in string will always be <see cref="Boolean.TrueString"/>.
		/// </summary>
		public bool IsFlag { get; set; }
		/// <summary>
		/// Indicates the argument is optional.
		/// </summary>
		public bool IsOptional { get; set; }
		/// <summary>
		/// Indicates that the setting cannot be null.
		/// </summary>
		public bool CannotBeNull { get; set; }
		/// <summary>
		/// Indicates whether or not the setting has been set yet.
		/// </summary>
		public bool HasBeenSet { get; protected set; }
		/// <summary>
		/// Indicates this is for providing help, and is not necessarily a setting.
		/// </summary>
		public bool IsHelp => Names.Any(x => x.CaseInsEquals("help") || x.CaseInsEquals("h"));

		/// <summary>
		/// The names of this command.
		/// </summary>
		public readonly ImmutableArray<string> Names;

		/// <summary>
		/// Creates an instance of <see cref="Setting"/>.
		/// </summary>
		/// <param name="names"></param>
		public Setting(IEnumerable<string> names)
		{
			if (names == null || !names.Any())
			{
				throw new ArgumentException("Must supply at least one name.");
			}

			Names = names.ToImmutableArray();
		}

		/// <summary>
		/// Sets the value back to its default value.
		/// </summary>
		public abstract void SetDefault();
		/// <summary>
		/// Converts the value to the required type and sets the property/field.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public abstract bool TrySetValue(string value, out string response);
		/// <summary>
		/// Returns the setting's name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Names[0];
		}
	}
}