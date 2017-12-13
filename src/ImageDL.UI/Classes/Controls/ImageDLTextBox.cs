using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows.Controls;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="TextBox"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLTextBox : TextBox, IFontResizeValue
	{
		private double _FRV;
		public double FontResizeValue
		{
			get => _FRV;
			set
			{
				_FRV = value;
				ElementUtils.SetFontResizeProperty(this, _FRV);
			}
		}
	}
}
