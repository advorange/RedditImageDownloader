using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Parses options and then sets them.
	/// </summary>
	/// <remarks>Reserved setting names: help, h</remarks>
	public sealed class SettingParser : ICollection<ISetting>
	{
		/// <summary>
		/// The default prefixes used for setting parsing.
		/// </summary>
		public static readonly ImmutableArray<string> DefaultPrefixes = new[] { "-", "--", "/" }.ToImmutableArray();

		/// <summary>
		/// Valid prefixes for a setting.
		/// </summary>
		public ImmutableArray<string> Prefixes { get; }
		/// <summary>
		/// Returns true if every setting has been set or is optional.
		/// </summary>
		/// <returns></returns>
		public bool AllSet => !_SettingMap.Values.Any(x => !(x.HasBeenSet || x.IsOptional));
		/// <inheritdoc />
		public bool IsReadOnly => false;
		/// <inheritdoc />
		public int Count => _SettingMap.Count;

		private readonly Dictionary<string, Guid> _NameMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<Guid, ISetting> _SettingMap = new Dictionary<Guid, ISetting>();

		/// <summary>
		/// Creates an instance of <see cref="SettingParser"/> with -, --, and / as valid prefixes and adds a help command.
		/// </summary>
		public SettingParser() : this(true, DefaultPrefixes.ToArray()) { }
		/// <summary>
		/// Creates an instance of <see cref="SettingParser"/>.
		/// </summary>
		/// <param name="addHelp"></param>
		/// <param name="prefixes"></param>
		public SettingParser(bool addHelp, params char[] prefixes) : this(addHelp, prefixes.Select(x => x.ToString()).ToArray()) { }
		/// <summary>
		/// Creates an instance of <see cref="SettingParser"/>.
		/// </summary>
		/// <param name="addHelp"></param>
		/// <param name="prefixes"></param>
		public SettingParser(bool addHelp, params string[] prefixes)
		{
			if (addHelp)
			{
				Add(new Setting<string>(new[] { "Help", "h" }, x => { })
				{
					Description = "Gives you help. Can't fix your life.",
					IsOptional = true,
				});
			}
			Prefixes = prefixes.ToImmutableArray();
		}

		/// <summary>
		/// Returns a dictionary of names and their values. Names are only counted if they begin with passed in a prefix.
		/// </summary>
		/// <param name="prefixes"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Dictionary<string, string> Parse(string[] prefixes, string input)
		{
			return Parse(prefixes, input.SplitLikeCommandLine());
		}
		/// <summary>
		/// Returns a dictionary of names and their values. Names are only counted if they begin with a passed in prefix.
		/// </summary>
		/// <param name="prefixes"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Dictionary<string, string> Parse(string[] prefixes, string[] input)
		{
			bool HasPrefix(string[] p, string i)
			{
				return p.Any(x => i.CaseInsStartsWith(x));
			}

			var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < input.Length; ++i)
			{
				var part = input[i];
				if (!HasPrefix(prefixes, part))
				{
					continue;
				}

				//Part following is not a setting, so it must be the value.
				if (input.Length - 1 > i && !HasPrefix(prefixes, input[i + 1]))
				{
					dict.Add(part, input[++i]);
				}
				//Otherwise there is no value
				else
				{
					dict.Add(part, null);
				}
			}
			return dict;
		}
		/// <summary>
		/// Finds settings and then sets their value.
		/// </summary>
		/// <param name="input"></param>
		public SettingParserResults Parse(string input)
		{
			return Parse(input.SplitLikeCommandLine());
		}
		/// <summary>
		/// Finds settings and then sets their value.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public SettingParserResults Parse(string[] input)
		{
			var unusedParts = new List<string>();
			var successes = new List<string>();
			var errors = new List<string>();
			var help = new List<string>();
			for (int i = 0; i < input.Length; ++i)
			{
				var part = input[i];
				string value;
				//No setting was gotten, so just skip this part
				if (!(GetSetting(part) is ISetting setting))
				{
					unusedParts.Add(part);
					continue;
				}

				//If it's a flag set its value to true then go to the next part
				if (setting.IsFlag)
				{
					value = (bool)setting.CurrentValue ? Boolean.FalseString : Boolean.TrueString;
				}
				//If there's one more and it's not a setting use that
				else if (input.Length - 1 > i && !(GetSetting(input[i + 1]) is ISetting throwaway))
				{
					value = input[++i]; //Make sure to increment i since the part is being used as a setting
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
			return new SettingParserResults(unusedParts, successes, errors, help);
		}
		/// <summary>
		/// Returns a string asking for unset settings.
		/// </summary>
		/// <returns></returns>
		public string GetNeededSettings()
		{
			var unsetArguments = _SettingMap.Values.Where(x => !(x.HasBeenSet || x.IsOptional));
			if (!unsetArguments.Any())
			{
				return $"Every setting which is necessary has been set.";
			}
			var sb = new StringBuilder("The following settings need to be set:" + Environment.NewLine);
			foreach (var setting in unsetArguments)
			{
				sb.AppendLine($"\t{setting.ToString()}");
			}
			return sb.ToString().Trim() + Environment.NewLine;
		}
		/// <summary>
		/// Gets the help information associated with this setting name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetHelp(string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				var vals = _SettingMap.Values.Select(x =>
				{
					return x.Names.Length < 2 ? x.Names[0] : $"{x.Names[0]} ({String.Join(", ", x.Names.Skip(1))})";
				});
				return $"All Settings:{Environment.NewLine}\t{String.Join($"{Environment.NewLine}\t", vals)}";
			}
			return GetSetting(name) is ISetting setting ? setting.Information : $"'{name}' is not a valid setting.";
		}
		/// <summary>
		/// Gets a setting with the supplied name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ISetting GetSetting(string name)
		{
			if (_NameMap.TryGetValue(name, out var guid))
			{
				return _SettingMap[guid];
			}
			foreach (var prefix in Prefixes)
			{
				if (!name.CaseInsStartsWith(prefix))
				{
					continue;
				}
				if (_NameMap.TryGetValue(name.Substring(prefix.Length), out guid))
				{
					return _SettingMap[guid];
				}
			}
			return null;
		}
		/// <inheritdoc />
		public void Add(ISetting setting)
		{
			var guid = Guid.NewGuid();
			foreach (var name in setting.Names)
			{
				_NameMap.Add(name, guid);
			}
			_SettingMap.Add(guid, setting);
		}
		/// <inheritdoc />
		public bool Remove(ISetting setting)
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
		public bool Contains(ISetting setting)
		{
			return _SettingMap.Values.Contains(setting);
		}
		/// <inheritdoc />
		public void Clear()
		{
			_NameMap.Clear();
			_SettingMap.Clear();
		}
		/// <inheritdoc />
		public void CopyTo(ISetting[] array, int index)
		{
			_SettingMap.Values.CopyTo(array, index);
		}
		/// <inheritdoc />
		public IEnumerator<ISetting> GetEnumerator()
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