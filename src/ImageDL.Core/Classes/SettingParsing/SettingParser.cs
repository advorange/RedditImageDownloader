using AdvorangesUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Parses options and then sets them.
	/// </summary>
	/// <remarks>Reserved setting names: help, h</remarks>
	public class SettingParser : Dictionary<string, Setting>
	{
		/// <summary>
		/// Valid prefixes for a setting.
		/// </summary>
		public readonly ImmutableArray<string> Prefixes;
		/// <summary>
		/// Returns true if every setting has been set or is optional.
		/// </summary>
		/// <returns></returns>
		public bool AllSet => !Values.Any(x => !(x.HasBeenSet || x.IsNotSetting));

		/// <summary>
		/// Creates an instance of <see cref="SettingParser"/>.
		/// </summary>
		/// <param name="prefixes"></param>
		public SettingParser(IEnumerable<string> prefixes) : base(StringComparer.OrdinalIgnoreCase)
		{
			Add(new Setting<string>(new[] { "help", "h" }, x => GetHelp(x)) { IsNotSetting = true, });

			Prefixes = prefixes.ToImmutableArray();
		}

		/// <summary>
		/// Adds the setting to the dictionary with each of its names as a key.
		/// </summary>
		/// <param name="setting"></param>
		public void Add(Setting setting)
		{
			foreach (var name in setting.Names)
			{
				Add(name, setting);
			}
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
					if (TryGetValue(part.Substring(prefix.Length), out var setting))
					{
						return setting;
					}
				}
				return null;
			}

			var unusedParts = new List<string>();
			var successes = new List<string>();
			var errors = new List<string>();
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
					value = true.ToString();
				}
				//If there's one more and it's not a setting use that
				else if (array.Length - 1 > i && !(GetSetting(array[i + 1]) is Setting throaway))
				{
					value = array[i + 1];
				}
				//Otherwise this part is unused
				else
				{
					unusedParts.Add(part);
					continue;
				}

				if (setting.TrySetValue(value, out var response))
				{
					successes.Add(response);
				}
				else
				{
					errors.Add(response);
				}
			}
			return new SettingsParseResult(unusedParts, successes, errors);
		}
		/// <summary>
		/// Returns the needed arguments all put into one string.
		/// </summary>
		/// <returns></returns>
		public string FormatNeededSettings()
		{
			if (AllSet)
			{
				return $"Every setting that is necessary has been set.";
			}

			var sb = new StringBuilder("The following settings need to be set:" + Environment.NewLine);
			foreach (var setting in Values.Distinct().Where(x => !(x.HasBeenSet || x.IsNotSetting)))
			{
				sb.AppendLine($"\t{setting.ToString()}");
			}
			return sb.ToString().Trim();
		}
		/// <summary>
		/// Gets the help information associated with this setting name.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string GetHelp(string input)
		{
			if (TryGetValue(input, out var setting))
			{
				return $"{input}: {setting.HelpString}";
			}
			else
			{
				return $"'{input}' is not a valid option.";
			}
		}
	}
}
