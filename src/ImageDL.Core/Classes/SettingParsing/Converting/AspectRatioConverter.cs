namespace ImageDL.Classes.SettingParsing.Converting
{
	/// <summary>
	/// A class for attempting to convert to aspect ratios.
	/// </summary>
	public class AspectRatioConverter : SettingConverter<AspectRatio>
	{
		/// <summary>
		/// Creates an instance of <see cref="AspectRatioConverter"/>.
		/// </summary>
		public AspectRatioConverter() : base(AspectRatio.TryParse) { }
	}
}
