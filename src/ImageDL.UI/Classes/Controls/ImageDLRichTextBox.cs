using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="RichTextBox"/> which implements some other useful properties and accepts custom colors easily.
	/// </summary>
	internal class ImageDLRichTextBox : RichTextBox, IFontResizeValue
	{
		public static readonly DependencyProperty FontResizeValueProperty = DependencyProperty.Register("FontResizeValue", typeof(double), typeof(ImageDLRichTextBox), new PropertyMetadata(ElementUtils.SetFontResizeProperty));
		public double FontResizeValue
		{
			get => (double)GetValue(FontResizeValueProperty);
			set => SetValue(FontResizeValueProperty, value);
		}
	}
}
