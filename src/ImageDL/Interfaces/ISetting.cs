using System;
using System.Collections.Immutable;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// An interface for settings.
	/// </summary>
	public interface ISetting
	{
		/// <summary>
		/// String indicating what this setting does.
		/// </summary>
		string Description { get; set; }
		/// <summary>
		/// String with information about the setting.
		/// </summary>
		string Information { get; }
		/// <summary>
		/// Indicates the setting is a boolean which only requires an attempt at parsing it for it to set itself to true.
		/// The passed in string will always be <see cref="Boolean.TrueString"/>.
		/// </summary>
		bool IsFlag { get; set; }
		/// <summary>
		/// Indicates the argument is optional.
		/// </summary>
		bool IsOptional { get; set; }
		/// <summary>
		/// Indicates that the setting cannot be null.
		/// </summary>
		bool CannotBeNull { get; set; }
		/// <summary>
		/// Indicates whether or not the setting has been set yet.
		/// </summary>
		bool HasBeenSet { get; }
		/// <summary>
		/// Indicates this is for providing help, and is not necessarily a setting.
		/// </summary>
		bool IsHelp { get; }
		/// <summary>
		/// The names of this command.
		/// </summary>
		ImmutableArray<string> Names { get; }

		/// <summary>
		/// Sets the value back to its default value.
		/// </summary>
		void SetDefault();
		/// <summary>
		/// Converts the value to the required type and sets the property/field.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		bool TrySetValue(string value, out string response);
		/// <summary>
		/// Returns the setting's name.
		/// </summary>
		/// <returns></returns>
		string ToString();
	}
}