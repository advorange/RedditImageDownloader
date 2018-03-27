using System;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Holds every primitive <see cref="SettingConverter{T}"/>.
	/// </summary>
	public static class PrimitiveSettingConverters
	{
#pragma warning disable 1591 //Self explanatory
		public static SettingConverter<sbyte> SbyteConverter => new SettingConverter<sbyte>(sbyte.TryParse);
		public static SettingConverter<byte> ByteConverter => new SettingConverter<byte>(byte.TryParse);
		public static SettingConverter<short> ShortConverter => new SettingConverter<short>(short.TryParse);
		public static SettingConverter<ushort> UshortConverter => new SettingConverter<ushort>(ushort.TryParse);
		public static SettingConverter<int> IntConverter => new SettingConverter<int>(int.TryParse);
		public static SettingConverter<uint> UintConverter => new SettingConverter<uint>(uint.TryParse);
		public static SettingConverter<long> LongConverter => new SettingConverter<long>(long.TryParse);
		public static SettingConverter<ulong> UlongConverter => new SettingConverter<ulong>(ulong.TryParse);
		public static SettingConverter<char> CharConverter => new SettingConverter<char>(char.TryParse);
		public static SettingConverter<float> FloatConverter => new SettingConverter<float>(float.TryParse);
		public static SettingConverter<double> DoubleConverter => new SettingConverter<double>(double.TryParse);
		public static SettingConverter<bool> BoolConverter => new SettingConverter<bool>(bool.TryParse);
		public static SettingConverter<decimal> DecimalConverter => new SettingConverter<decimal>(decimal.TryParse);
		/// <summary>
		/// It makes a lot of things easier even though it is stupid.
		/// </summary>
		public static SettingConverter<string> StringConverter => new SettingConverter<string>(StringTryParse);
#pragma warning restore 1591

		/// <summary>
		/// Attempts to get a primitive converter based on the passed in generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static SettingConverter GetConverter<T>()
		{
			return GetConverter(typeof(T));
		}
		/// <summary>
		/// Attempts to get a primitive converter based on the passed in type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static SettingConverter GetConverter(Type type)
		{
			switch (type.Name)
			{
				case nameof(SByte):
					return SbyteConverter;
				case nameof(Byte):
					return ByteConverter;
				case nameof(Int16):
					return ShortConverter;
				case nameof(UInt16):
					return UshortConverter;
				case nameof(Int32):
					return IntConverter;
				case nameof(UInt32):
					return UintConverter;
				case nameof(Int64):
					return LongConverter;
				case nameof(UInt64):
					return UlongConverter;
				case nameof(Char):
					return CharConverter;
				case nameof(Single):
					return FloatConverter;
				case nameof(Double):
					return DoubleConverter;
				case nameof(Boolean):
					return BoolConverter;
				case nameof(Decimal):
					return DecimalConverter;
				case nameof(String):
					return StringConverter;
				default:
					throw new ArgumentException($"Unable to find a primitive converter for the supplied type {type.Name}.");
			}
		}

		private static bool StringTryParse(string s, out string result)
		{
			result = s;
			return true;
		}
	}
}
