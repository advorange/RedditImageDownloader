using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="Button"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLButton : Button, IFontResizeValue
	{
		public static readonly DependencyProperty FontResizeValueProperty = DependencyProperty.Register("FontResizeValue", typeof(double), typeof(ImageDLButton), new PropertyMetadata(ElementUtils.SetFontResizeProperty));
		public double FontResizeValue
		{
			get => (double)GetValue(FontResizeValueProperty);
			set => SetValue(FontResizeValueProperty, value);
		}
	}
}
