using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// A generic class for option, specifying what the option type is.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class Setting<T> : ISetting
	{
		/// <inheritdoc />
		public string Description { get; set; }
		/// <inheritdoc />
		public string Information
		{
			get
			{
				var help = $"{Names[0]}: {Description}";
				if (!EqualityComparer<T>.Default.Equals(DefaultValue, default))
				{
					help += $" Default value is {DefaultValue}.";
				}
				if (typeof(T).IsEnum)
				{
					help += $"{Environment.NewLine}Acceptable values: {String.Join(", ", Enum.GetNames(typeof(T)))}";
				}
				return help;
			}
		}
		/// <inheritdoc />
		public bool IsFlag { get; set; }
		/// <inheritdoc />
		public bool IsOptional { get; set; }
		/// <inheritdoc />
		public bool CannotBeNull { get; set; }
		/// <inheritdoc />
		public bool HasBeenSet { get; private set; }
		/// <inheritdoc />
		public bool IsHelp => Names.Any(x => x.CaseInsEquals("help") || x.CaseInsEquals("h"));
		/// <inheritdoc />
		public ImmutableArray<string> Names { get; }
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
		public Setting(IEnumerable<string> names, Action<T> setter, TryParseDelegate<T> parser = default)
		{
			if (names == null || !names.Any())
			{
				throw new ArgumentException("Must supply at least one name.");
			}

			Names = names.ToImmutableArray();
			_Setter = setter ?? throw new ArgumentException("Invalid setter supplied.");
			_Parser = parser ?? GetPrimitiveParser();
		}

		/// <inheritdoc />
		public void SetDefault()
		{
			_Setter(DefaultValue);
		}
		/// <inheritdoc />
		public bool TrySetValue(string value, out string response)
		{
			T result = default;
			if (value.CaseInsEquals("default")) //Let default just pass on through
			{
				result = default;
			}
			else
			{
				var (Success, Value) = _Parser(value);
				if (!Success)
				{
					response = $"Unable to convert '{value}' to type {typeof(T).Name}.";
					return false;
				}
				result = Value;
			}
			if (result == null && CannotBeNull)
			{
				response = $"{Names[0]} cannot be set to 'NULL'.";
				return false;
			}

			try
			{
				_Setter(result);
			}
			//Catch all because who knows what exceptions will happen, and it's user input
			catch (Exception e)
			{
				response = e.Message;
				return false;
			}
			HasBeenSet = true;
			response = $"Successfully set {Names[0]} to '{result?.ToString() ?? "NULL"}'.";
			return true;
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
					return (TryParseDelegate<T>)(object)new TryParseDelegate<sbyte>(s => (sbyte.TryParse(s, out var result), result));
				case nameof(Byte):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<byte>(s => (byte.TryParse(s, out var result), result));
				case nameof(Int16):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<short>(s => (short.TryParse(s, out var result), result));
				case nameof(UInt16):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<ushort>(s => (ushort.TryParse(s, out var result), result));
				case nameof(Int32):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<int>(s => (int.TryParse(s, out var result), result));
				case nameof(UInt32):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<uint>(s => (uint.TryParse(s, out var result), result));
				case nameof(Int64):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<long>(s => (long.TryParse(s, out var result), result));
				case nameof(UInt64):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<ulong>(s => (ulong.TryParse(s, out var result), result));
				case nameof(Char):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<char>(s => (char.TryParse(s, out var result), result));
				case nameof(Single):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<float>(s => (float.TryParse(s, out var result), result));
				case nameof(Double):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<double>(s => (double.TryParse(s, out var result), result));
				case nameof(Boolean):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<bool>(s => (bool.TryParse(s, out var result), result));
				case nameof(Decimal):
					return (TryParseDelegate<T>)(object)new TryParseDelegate<decimal>(s => (decimal.TryParse(s, out var result), result));
				case nameof(String):
					//Instead of having to do special checks, just use this dumb delegate
					return (TryParseDelegate<T>)(object)new TryParseDelegate<string>(s => (StringTryParse(s, out var result), result));
				default:
					throw new ArgumentException($"Unable to find a primitive converter for the supplied type {typeof(T).Name}.");
			}
		}
	}
}