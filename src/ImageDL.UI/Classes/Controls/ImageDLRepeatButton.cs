using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="RepeatButton"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLRepeatButton : RepeatButton, IFontResizeValue
	{
		public static readonly DependencyProperty FontResizeValueProperty = DependencyProperty.Register("FontResizeValue", typeof(double), typeof(ImageDLRepeatButton), new PropertyMetadata(ElementUtils.SetFontResizeProperty));
		public double FontResizeValue
		{
			get => (double)GetValue(FontResizeValueProperty);
			set => SetValue(FontResizeValueProperty, value);
		}
	}
}
