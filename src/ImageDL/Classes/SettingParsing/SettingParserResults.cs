using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Holds the results of parsing settings.
	/// </summary>
	public struct SettingParserResults
	{
		/// <summary>
		/// Parts that were not used to set something.
		/// </summary>
		public readonly ImmutableArray<string> UnusedParts;
		/// <summary>
		/// All successfully set settings.
		/// </summary>
		public readonly ImmutableArray<string> Successes;
		/// <summary>
		/// Any errors which occurred when setting something.
		/// </summary>
		public readonly ImmutableArray<string> Errors;
		/// <summary>
		/// Result gotten via the help setting.
		/// </summary>
		public readonly ImmutableArray<string> Help;
		/// <summary>
		/// Returns true if <see cref="UnusedParts"/> and <see cref="Errors"/> are both empty.
		/// </summary>
		public readonly bool IsSuccess;

		/// <summary>
		/// Creates an instance of <see cref="SettingParserResults"/>.
		/// </summary>
		/// <param name="unusedParts"></param>
		/// <param name="successes"></param>
		/// <param name="errors"></param>
		/// <param name="help"></param>
		public SettingParserResults(IEnumerable<string> unusedParts, IEnumerable<string> successes, IEnumerable<string> errors, IEnumerable<string> help)
		{
			UnusedParts = (unusedParts ?? Enumerable.Empty<string>()).Where(x => x != null).ToImmutableArray();
			Successes = (successes ?? Enumerable.Empty<string>()).Where(x => x != null).ToImmutableArray();
			Errors = (errors ?? Enumerable.Empty<string>()).Where(x => x != null).ToImmutableArray();
			Help = (help ?? Enumerable.Empty<string>()).Where(x => x != null).ToImmutableArray();
			IsSuccess = !UnusedParts.Any() && !Errors.Any();
		}
		
		/// <inheritdoc />
		public override string ToString()
		{
			var responses = new List<string>();
			if (Help.Any())
			{
				responses.Add(String.Join("\n", Help));
			}
			if (Successes.Any())
			{
				responses.Add(String.Join("\n", Successes));
			}
			if (Errors.Any())
			{
				responses.Add($"The following errors occurred:\n{String.Join("\n\t", Errors)}");
			}
			if (UnusedParts.Any())
			{
				responses.Add($"The following parts were extra; was an argument mistyped? '{String.Join("', '", UnusedParts)}'");
			}
			return String.Join("\n", responses) + "\n";
		}
	}
}