using AdvorangesUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Parses options and then sets them.
	/// </summary>
	/// <remarks>Reserved setting names: help, h</remarks>
	public class SettingParser : IEnumerable<Setting>
	{
		/// <summary>
		/// Valid prefixes for a setting.
		/// </summary>
		public readonly ImmutableArray<string> Prefixes;
		/// <summary>
		/// Returns true if every setting has been set or is optional.
		/// </summary>
		/// <returns></returns>
		public bool AllSet => !GetNeededSettings().Any();

		private readonly Dictionary<string, Guid> _NameMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<Guid, Setting> _SettingMap = new Dictionary<Guid, Setting>();

		/// <summary>
		/// Creates an instance of <see cref="SettingParser"/>.
		/// </summary>
		/// <param name="prefixes"></param>
		public SettingParser(IEnumerable<string> prefixes)
		{
			Prefixes = prefixes.ToImmutableArray();
			Add(new Setting<string>(new[] { "Help", "h" }, x => { })
			{
				Description = "Gives you help. Can't fix your life.",
				IsOptional = true,
			});
		}

		/// <summary>
		/// Finds settings and then sets their value. Returns unused parts.
		/// </summary>
		/// <param name="parts"></param>
		public SettingsParseResult Parse(IEnumerable<string> parts)
		{
			//Try to find the setting, will only use the first match, even if there are multiple matches
			Setting GetSetting(string part)
			{
				foreach (var prefix in Prefixes)
				{
					if (!part.CaseInsStartsWith(prefix))
					{
						continue;
					}
					if (_NameMap.TryGetValue(part.Substring(prefix.Length), out var guid))
					{
						return _SettingMap[guid];
					}
				}
				return null;
			}

			var unusedParts = new List<string>();
			var successes = new List<string>();
			var errors = new List<string>();
			var help = new List<string>();
			var array = parts.ToArray();
			for (int i = 0; i < array.Length; ++i)
			{
				var part = array[i];
				string value;
				//No setting was gotten, so just skip this part
				if (!(GetSetting(part) is Setting setting))
				{
					unusedParts.Add(part);
					continue;
				}
				//If it's a flag set its value to true then go to the next part
				else if (setting.IsFlag)
				{
					value = Boolean.TrueString;
				}
				//If there's one more and it's not a setting use that
				else if (array.Length - 1 > i && !(GetSetting(array[i + 1]) is Setting throwaway))
				{
					value = array[++i]; //Make sure to increment i since the part is being used as a setting
				}
				//If help and had argument, would have gone into the above statement.
				//This means it has gotten to the flag aspect of it, so null can just be passed in.
				else if (setting.IsHelp)
				{
					value = null;
				}
				//Otherwise this part is unused
				else
				{
					unusedParts.Add(part);
					continue;
				}

				if (setting.IsHelp)
				{
					help.Add(GetHelp(value));
				}
				else if (setting.TrySetValue(value, out var response))
				{
					successes.Add(response);
				}
				else
				{
					errors.Add(response);
				}
			}
			return new SettingsParseResult(unusedParts, successes, errors, help);
		}
		/// <summary>
		/// Returns the needed arguments all put into one string.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Setting> GetNeededSettings()
		{
			return _SettingMap.Values.Where(x => !(x.HasBeenSet || x.IsOptional));
		}
		/// <summary>
		/// Gets the help information associated with this setting name.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string GetHelp(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
			{
				var vals = _SettingMap.Values.Select(x =>
				{
					return x.Names.Length < 2 ? x.Names[0] : $"{x.Names[0]} ({String.Join(", ", x.Names.Skip(1))})";
				});
				return $"All Settings:\n\t{String.Join("\n\t", vals)}";
			}
			else if (_NameMap.TryGetValue(input, out var guid))
			{
				return _SettingMap[guid].GetHelp();
			}
			else
			{
				return $"'{input}' is not a valid setting.";
			}
		}
		/// <summary>
		/// Adds the setting to the dictionary and maps it by its names.
		/// </summary>
		/// <param name="setting"></param>
		public void Add(Setting setting)
		{
			var guid = Guid.NewGuid();
			foreach (var name in setting.Names)
			{
				_NameMap.Add(name, guid);
			}
			_SettingMap.Add(guid, setting);
		}
		/// <summary>
		/// Removes the setting from the dictionary and unmaps all its names.
		/// </summary>
		/// <param name="setting"></param>
		/// <returns></returns>
		public bool Remove(Setting setting)
		{
			if (!_NameMap.TryGetValue(setting.Names[0], out var guid))
			{
				return false;
			}
			foreach (var name in setting.Names)
			{
				_NameMap.Remove(name);
			}
			return _SettingMap.Remove(guid);
		}
		/// <inheritdoc />
		public IEnumerator<Setting> GetEnumerator()
		{
			return _SettingMap.Values.GetEnumerator();
		}
		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _SettingMap.Values.GetEnumerator();
		}
	}
}