using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageDL.UI.Classes
{
	public class DpiDecorator : Decorator
	{
		public DpiDecorator()
		{
			Loaded += (sender, e) =>
			{
				var matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
				var dpiTransform = new ScaleTransform(1 / matrix.M11, 1 / matrix.M22);
				if (dpiTransform.CanFreeze)
				{
					dpiTransform.Freeze();
				}
				LayoutTransform = dpiTransform;
			};
		}
	}
}
