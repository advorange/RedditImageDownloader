using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageDL.UI.Classes
{
	internal class FontResizeConverter : IValueConverter
	{
		private double _ConvertFactor;
		public double ConvertFactor
		{
			get => _ConvertFactor;
			set
			{
				if (_ConvertFactor < 0)
				{
					throw new ArgumentException($"{nameof(ConvertFactor)} must be greater than or equal to 0.");
				}
				_ConvertFactor = value;
			}
		}

		public FontResizeConverter()
		{
			ConvertFactor = .02;
		}
		public FontResizeConverter(double convertFactor)
		{
			ConvertFactor = convertFactor;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var converted = System.Convert.ToInt32(value);
			if (converted == 0)
			{
				converted = 1;
			}
			return ConvertFactor == 0 ? converted : converted * ConvertFactor;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var converted = System.Convert.ToInt32(value);
			return ConvertFactor == 0 ? converted : converted / ConvertFactor;
		}
	}
}
