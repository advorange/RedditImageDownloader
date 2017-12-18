using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System.Windows.Controls.Primitives;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="Button"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLRepeatButton : RepeatButton, IFontResizeValue
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
