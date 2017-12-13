using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="RichTextBox"/> which implements some other useful properties and accepts custom colors easily.
	/// </summary>
	internal class ImageDLRichTextBox : RichTextBox, IFontResizeValue
	{
		private double _FRV;
		public double FontResizeValue
		{
			get => _FRV;
			set
			{
				ElementUtils.SetFontResizeProperty(this, value);
				_FRV = value;
			}
		}
	}
}
