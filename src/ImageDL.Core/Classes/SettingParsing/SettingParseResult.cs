using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImageDL.Classes.SettingParsing
{
	/// <summary>
	/// Holds the results of parsing settings.
	/// </summary>
	public struct SettingsParseResult
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
		/// Creates an instance of <see cref="SettingsParseResult"/>.
		/// </summary>
		/// <param name="unusedParts"></param>
		/// <param name="successes"></param>
		/// <param name="errors"></param>
		/// <param name="help"></param>
		public SettingsParseResult(IEnumerable<string> unusedParts, IEnumerable<string> successes, IEnumerable<string> errors, IEnumerable<string> help)
		{
			UnusedParts = (unusedParts ?? Enumerable.Empty<string>()).ToImmutableArray();
			Successes = (successes ?? Enumerable.Empty<string>()).ToImmutableArray();
			Errors = (errors ?? Enumerable.Empty<string>()).ToImmutableArray();
			Help = (help ?? Enumerable.Empty<string>()).ToImmutableArray();
			IsSuccess = !UnusedParts.Any() && !Errors.Any();
		}
	}
}