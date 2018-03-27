namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Delegate used to try parse something.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="s"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public delegate bool TryParseDelegate<T>(string s, out T result);
}
