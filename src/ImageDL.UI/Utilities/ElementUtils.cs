using ImageDL.UI.Classes;
using ImageDL.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageDL.UI.Utilities
{
	internal static class ElementUtils
	{
		/// <summary>
		/// Sets the <see cref="Grid.RowSpanProperty"/> to either 1 or the supplied length.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="length"></param>
		public static void SetRowSpan(UIElement item, int length) => Grid.SetRowSpan(item, Math.Max(1, length));
		/// <summary>
		/// Sets the <see cref="Grid.ColumnSpanProperty"/> to either 1 or the supplied length.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="length"></param>
		public static void SetColSpan(UIElement item, int length) => Grid.SetColumnSpan(item, Math.Max(1, length));
		/// <summary>
		/// Sets the <see cref="Control.FontSizeProperty"/> to a number that changes based off of the top most grid's height.
		/// Zero removes any binding on the <see cref="Control.FontSizeProperty"/>.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="size"></param>
		/// <exception cref="ArgumentException">If <paramref name="control"/> is not inside a grid.</exception>
		/// <exception cref="ArgumentException">If <paramref name="size"/> is less than zero.</exception>
		public static void SetFontResizeProperty(Control control, double size)
		{
			if (!TryGetTopMostParent(control, out Grid parent, out int ancestorLevel))
			{
				throw new ArgumentException($"{control.Name} must be inside a grid if {nameof(IFontResizeValue.FontResizeValue)} is set.");
			}
			else if (size < 0)
			{
				throw new ArgumentException($"{nameof(size)} must be greater than or equal to 0.");
			}
			else if (size == 0)
			{
				BindingOperations.ClearBinding(control, Control.FontSizeProperty);
				return;
			}

			control.SetBinding(Control.FontSizeProperty, new Binding
			{
				Path = new PropertyPath("ActualHeight"),
				RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Grid), ancestorLevel),
				Converter = new FontResizeConverter(size),
			});
		}

		/// <summary>
		/// Returns true if the supplied type is any parent of the supplied element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="element"></param>
		/// <param name="parent"></param>
		/// <param name="ancestorLevel"></param>
		/// <returns></returns>
		public static bool TryGetTopMostParent<T>(this DependencyObject element, out T parent, out int ancestorLevel) where T : DependencyObject
		{
			parent = null;
			ancestorLevel = 0;

			var currElement = element;
			var currLevel = 0;
			while (true)
			{
				currElement = VisualTreeHelper.GetParent(currElement);
				if (currElement is T tParent)
				{
					parent = tParent;
					ancestorLevel = ++currLevel;
				}
				else if (currElement == null)
				{
					break;
				}
			}
			return ancestorLevel > 0;
		}
		/// <summary>
		/// Returns every child <paramref name="parent"/> has.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static IEnumerable<DependencyObject> GetChildren(this DependencyObject parent)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
			{
				yield return VisualTreeHelper.GetChild(parent, i);
			}
		}
	}
}
