namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Attempts to convert the string to the supplied type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="s"></param>
	/// <returns></returns>
	public delegate (bool Success, T Value) TryParseDelegate<T>(string s);
}