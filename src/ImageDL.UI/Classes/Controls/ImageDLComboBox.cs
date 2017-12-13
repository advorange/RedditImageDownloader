using ImageDL.UI.Interfaces;
using ImageDL.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="ComboBox"/> which implements some other useful properties.
	/// </summary>
	internal class ImageDLComboBox : ComboBox, IFontResizeValue
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
		private Type _SET;
		public Type SourceEnumType
		{
			get => _SET;
			set
			{
				ItemsSource = CreateItemsSourceOutOfEnum(value);
				_SET = value;
			}
		}

		public ImageDLComboBox()
		{
			VerticalContentAlignment = VerticalAlignment.Center;
			HorizontalContentAlignment = HorizontalAlignment.Center;
		}

		/// <summary>
		/// Returns the values of <see cref="CreateItemsSourceOutOfEnum(Type)"/> by passing in <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<TextBox> CreateItemsSourceOutOfEnum<T>() where T : struct, IConvertible, IComparable, IFormattable
			=> CreateItemsSourceOutOfEnum(typeof(T));
		/// <summary>
		/// Returns textboxes with the text as the enum name and the tag as the enum.
		/// </summary>
		/// <param name="enumType"></param>
		/// <returns></returns>
		public static IEnumerable<TextBox> CreateItemsSourceOutOfEnum(Type enumType)
		{
			foreach (var e in Enum.GetValues(enumType) ?? throw new ArgumentException($"{nameof(enumType)} must be an enum."))
			{
				yield return CreateItem(Enum.GetName(enumType, e), e);
			}
		}
		/// <summary>
		/// Returns textboxes with the text as the string and the tag as the string too.
		/// </summary>
		/// <param name="strings"></param>
		/// <returns></returns>
		public static IEnumerable<TextBox> CreateComboBoxSourceOutOfStrings(params string[] strings)
		{
			foreach (var s in strings)
			{
				yield return CreateItem(s, s);
			}
		}
		private static TextBox CreateItem(string text, object tag)
			=> new ImageDLTextBox
			{
				FontFamily = new FontFamily("Courier New"),
				Text = text,
				Tag = tag,
				IsReadOnly = true,
				IsHitTestVisible = false,
				BorderThickness = new Thickness(0),
				Background = Brushes.Transparent,
				Foreground = Brushes.Black,
			};
	}
}
