using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RedditImageDownloader.Classes
{
	public class Properties
	{
		//TODO: go back to the font resizing from before
		public static readonly DependencyProperty FontResizeProperty;

		static Properties()
		{
			FontResizeProperty = DependencyProperty.RegisterAttached("MyInt", typeof(int), typeof(Properties));
		}

		public static void SetFontResizeProperty(UIElement element, int value) => element.SetValue(FontResizeProperty, value);
		public static int GetFontResizeProperty(UIElement element) => (int)element.GetValue(FontResizeProperty);
	}
}
