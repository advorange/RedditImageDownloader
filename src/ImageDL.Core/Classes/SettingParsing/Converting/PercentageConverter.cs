namespace ImageDL.Classes.SettingParsing.Converting
{
	/// <summary>
	/// A class for attempting to convert to percentages.
	/// </summary>
	public class PercentageConverter : SettingConverter<Percentage>
	{
		/// <summary>
		/// Creates an instance of <see cref="PercentageConverter"/>.
		/// </summary>
		public PercentageConverter() : base(Percentage.TryParse) { }
	}
}