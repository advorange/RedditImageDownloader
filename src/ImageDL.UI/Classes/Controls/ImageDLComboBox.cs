using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="ComboBox"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLComboBox : ComboBox, IFontResizeValue
	{
		public static readonly DependencyProperty FontResizeValueProperty = DependencyProperty.Register("FontResizeValue", typeof(double), typeof(ImageDLComboBox), new PropertyMetadata(ElementUtils.SetFontResizeProperty));
		public double FontResizeValue
		{
			get => (double)GetValue(FontResizeValueProperty);
			set => SetValue(FontResizeValueProperty, value);
		}

		public ImageDLComboBox()
		{
			VerticalContentAlignment = VerticalAlignment.Center;
			HorizontalContentAlignment = HorizontalAlignment.Center;
		}
	}
}
