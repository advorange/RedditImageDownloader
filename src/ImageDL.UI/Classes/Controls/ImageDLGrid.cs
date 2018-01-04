using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="Grid"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLGrid : Grid, IFontResizeValue
	{
		public static readonly DependencyProperty FontResizeValueProperty = DependencyProperty.Register("FontResizeValue", typeof(double), typeof(ImageDLGrid), new PropertyMetadata(ElementUtils.SetFontResizeProperty));
		public double FontResizeValue
		{
			get => (double)GetValue(FontResizeValueProperty);
			set => SetValue(FontResizeValueProperty, value);
		}

		public override void EndInit()
		{
			base.EndInit();
			if (FontResizeValue > 0)
			{
				SetAllChildrenToFontSizeProperty(this);
			}
		}

		private void SetAllChildrenToFontSizeProperty(DependencyObject parent)
		{
			foreach (var child in parent.GetChildren())
			{
				//Don't set it on controls with it already set
				if (child is IFontResizeValue frv && frv.FontResizeValue == default)
				{
					frv.FontResizeValue = FontResizeValue;
				}
				SetAllChildrenToFontSizeProperty(child);
			}
		}
	}
}
