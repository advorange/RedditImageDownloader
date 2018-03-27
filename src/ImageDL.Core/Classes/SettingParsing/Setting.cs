using AdvorangesUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// A generic class for option, specifying what the option type is.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Setting<T> : Setting
	{
		/// <summary>
		/// Default value of the setting.
		/// </summary>
		public T DefaultValue
		{
			get => _DefaultValue;
			set
			{
				_DefaultValue = value;
				HasBeenSet = true;
				SetDefault();
			}
		}

		private readonly SettingConverter<T> Converter;
		private readonly Action<T> Setter;
		private T _DefaultValue;

		/// <summary>
		/// Creates an instance of <see cref="Setting{T}"/>.
		/// </summary>
		/// <param name="names">The names to use for this option. Must supply at least one name. The first name will be designated the main name.</param>
		/// <param name="setter">The setter to use for this option.</param>
		/// <param name="converter">The converter to convert from a string to the value. Can be null if a primitive type.</param>
		public Setting(IEnumerable<string> names, Action<T> setter, SettingConverter<T> converter = default) : base(names)
		{
			Setter = setter ?? throw new ArgumentException("Invalid setter supplied.");
			Converter = converter ?? (SettingConverter<T>)PrimitiveSettingConverters.GetConverter<T>();
		}

		/// <inheritdoc />
		public override void SetDefault()
		{
			Setter(DefaultValue);
		}
		/// <inheritdoc />
		public override bool TrySetValue(string value, out string response)
		{
			T result = default;
			if (value.CaseInsEquals("default")) //Let default just pass on through
			{
				result = default;
			}
			else if (!Converter.TryConvert(value, out result, out response))
			{
				return false;
			}
			else if (result == null && CannotBeNull)
			{
				response = $"{Names[0]} cannot be set to 'NULL'.";
				return false;
			}

			Setter(result);
			HasBeenSet = true;
			response = $"Successfully set {Names[0]} to '{result?.ToString() ?? "NULL"}'.";
			return true;
		}
		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Names[0]} ({typeof(T).Name})";
		}
	}

	/// <summary>
	/// An abstract class for options.
	/// </summary>
	public abstract class Setting
	{
		/// <summary>
		/// The names of settings that this setting conflicts with.
		/// </summary>
		public List<string> ConflictsWith { get; set; }
		/// <summary>
		/// String indicating what this setting does.
		/// </summary>
		public string HelpString { get; set; }
		/// <summary>
		/// Indicates the setting is a boolean which only requires an attempt at parsing it for it to set itself to true.
		/// </summary>
		public bool IsFlag { get; set; }
		/// <summary>
		/// Seems like a dumb setting, but is used for the help command.
		/// </summary>
		public bool IsNotSetting { get; set; }
		/// <summary>
		/// Indicates that the setting cannot be null.
		/// </summary>
		public bool CannotBeNull { get; set; }
		/// <summary>
		/// Indicates whether or not the setting has been set yet.
		/// </summary>
		public bool HasBeenSet { get; protected set; }

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
		/// <inheritdoc />
		public override string ToString()
		{
			return Names[0];
		}
	}
}
