namespace ImageDL.Classes.SettingParsing.Converting
{
	/// <summary>
	/// A class for attempting to convert values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SettingConverter<T> : SettingConverter
	{
		private TryParseDelegate<T> TryParser;

		/// <summary>
		/// Creates an instance of <see cref="SettingConverter{T}"/>.
		/// </summary>
		/// <param name="tryParser"></param>
		public SettingConverter(TryParseDelegate<T> tryParser)
		{
			TryParser = tryParser;
		}

		/// <inheritdoc />
		public override bool TryConvertObject(string s, out object result, out string error)
		{
			var val = TryConvert(s, out var converted, out error);
			result = converted;
			return val;
		}
		/// <summary>
		/// Attempts to parse the string for the type. If fails, returns an error saying that it failed.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public virtual bool TryConvert(string s, out T result, out string error)
		{
			return (error = TryParser(s, out result) ? null : $"Unable to convert {s} to type {typeof(T).Name}.") == null;
		}
	}

	/// <summary>
	/// An abstract implementation for <see cref="SettingConverter{T}"/>.
	/// </summary>
	public abstract class SettingConverter
	{
		/// <summary>
		/// Attempts to convert the passed in string to the supplied type.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public abstract bool TryConvertObject(string s, out object result, out string error);
	}
}
