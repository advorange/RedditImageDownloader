using ImageDL.Classes.SettingParsing;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that has settings.
	/// </summary>
	public interface IHasSettings
	{
		/// <summary>
		/// Used to set arguments via command line.
		/// </summary>
		SettingParser SettingParser { get; }
		/// <summary>
		/// Indicates that all arguments have been set and that the user wants the downloader to start.
		/// </summary>
		bool CanStart { get; }
	}
}