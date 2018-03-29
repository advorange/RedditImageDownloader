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
	public sealed class Setting<T> : Setting
	{
		/// <summary>
		/// Default value of the setting. This will indicate the setting is optional, but has a value other than the default value of the type.
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

		private readonly TryParseDelegate<T> _Parser;
		private readonly Action<T> _Setter;
		private T _DefaultValue;

		/// <summary>
		/// Creates an instance of <see cref="Setting{T}"/>.
		/// </summary>
		/// <param name="names">The names to use for this option. Must supply at least one name. The first name will be designated the main name.</param>
		/// <param name="setter">The setter to use for this option.</param>
		/// <param name="parser">The converter to convert from a string to the value. Can be null if a primitive type.</param>
		public Setting(IEnumerable<string> names, Action<T> setter, TryParseDelegate<T> parser = default) : base(names)
		{
			_Setter = setter ?? throw new ArgumentException("Invalid setter supplied.");
			_Parser = parser ?? GetPrimitiveParser();
		}

		/// <inheritdoc />
		public override void SetDefault()
		{
			_Setter(DefaultValue);
		}
		/// <inheritdoc />
		public override bool TrySetValue(string value, out string response)
		{
			T result = default;
			if (value.CaseInsEquals("default")) //Let default just pass on through
			{
				result = default;
			}
			else if (!_Parser(value, out result))
			{
				response = $"Unable to convert '{value}' to type {typeof(T).Name}.";
				return false;
			}
			else if (result == null && CannotBeNull)
			{
				response = $"{Names[0]} cannot be set to 'NULL'.";
				return false;
			}

			_Setter(result);
			HasBeenSet = true;
			response = $"Successfully set {Names[0]} to '{result?.ToString() ?? "NULL"}'.";
			return true;
		}
		/// <inheritdoc />
		public override string GetHelp()
		{
			var defaultValue = EqualityComparer<T>.Default.Equals(DefaultValue, default) ? "" : $" Default value is {DefaultValue}.";
			return $"{Names[0]}: {Description}{defaultValue}";
		}
		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Names[0]} ({typeof(T).Name})";
		}

		private TryParseDelegate<T> GetPrimitiveParser()
		{
			bool StringTryParse(string s, out string result)
			{
				result = s;
				return true;
			}

			switch (typeof(T).Name)
			{
				case nameof(SByte):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<sbyte>(sbyte.TryParse);
				case nameof(Byte):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<byte>(byte.TryParse);
				case nameof(Int16):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<short>(short.TryParse);
				case nameof(UInt16):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<ushort>(ushort.TryParse);
				case nameof(Int32):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<int>(int.TryParse);
				case nameof(UInt32):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<uint>(uint.TryParse);
				case nameof(Int64):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<long>(long.TryParse);
				case nameof(UInt64):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<ulong>(ulong.TryParse);
				case nameof(Char):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<char>(char.TryParse);
				case nameof(Single):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<float>(float.TryParse);
				case nameof(Double):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<double>(double.TryParse);
				case nameof(Boolean):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<bool>(bool.TryParse);
				case nameof(Decimal):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<decimal>(decimal.TryParse);
				case nameof(String):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<string>(StringTryParse);
				default:
					throw new ArgumentException($"Unable to find a primitive converter for the supplied type {typeof(T).Name}.");
			}
		}

		/// <summary>
		/// Delegate used to try parse something.
		/// </summary>
		/// <typeparam name="TConvert"></typeparam>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public delegate bool TryParseDelegate<TConvert>(string s, out TConvert result);
	}

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
		/// Returns a string with additional information about the setting.
		/// </summary>
		/// <returns></returns>
		public abstract string GetHelp();
		/// <inheritdoc />
		public override string ToString()
		{
			return Names[0];
		}
	}
}