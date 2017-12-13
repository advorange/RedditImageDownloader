using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageDL.UI.Classes
{
	internal class FontResizeConverter : IValueConverter
	{
		private double _ConvertFactor;

		public FontResizeConverter(double convertFactor)
		{
			_ConvertFactor = convertFactor;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> Math.Max((int)(System.Convert.ToInt16(value) * _ConvertFactor), 1);
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
