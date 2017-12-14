using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace ImageDL.UI.Utilities
{
	internal static class BrushUtils
	{
		private static Dictionary<string, SolidColorBrush> _Brushes = typeof(Brushes)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(p => p.PropertyType == typeof(SolidColorBrush))
			.ToDictionary(p => p.Name, p => (SolidColorBrush)p.GetValue(null),
			StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Returns a brush. Can be created with R/G/B, name, or hex.
		/// </summary>
		/// <param name="input">The string to create a brush from.</param>
		/// <returns>A brush with the supplied color.</returns>
		public static SolidColorBrush CreateBrush(string input)
		{
			if (TryCreateBrushFromStringRGB(input.Split('/'), out var rgb))
			{
				return rgb;
			}
			else if (TryCreateBrushFromStringName(input, out var name))
			{
				return name;
			}
			else if (TryCreateBrushFromStringHex(input, out var hex))
			{
				return hex;
			}
			else
			{
				return default;
			}
		}
		/// <summary>
		/// Does the same as <see cref="CreateBrush(string)"/> except is in TryCreate format.
		/// Returns true if a brush was successfully created.
		/// </summary>
		/// <param name="input">The string to create a brush from.</param>
		/// <param name="brush">The created brush.</param>
		/// <returns>A boolean indicating whether or not the brush was successfully created.</returns>
		public static bool TryCreateBrush(string input, out SolidColorBrush brush)
		{
			if (TryCreateBrushFromStringRGB(input.Split('/'), out var rgb))
			{
				brush = rgb;
			}
			else if (TryCreateBrushFromStringName(input, out var name))
			{
				brush = name;
			}
			else if (TryCreateBrushFromStringHex(input, out var hex))
			{
				brush = hex;
			}
			else
			{
				brush = default;
				return false;
			}
			return true;
		}
		/// <summary>
		/// Creates a brush from an array of strings. The elements should be in the order of R, then G, then B.
		/// Returns true if a brush was successfully created.
		/// </summary>
		/// <param name="rgb">The strings to create a brush from.</param>
		/// <param name="brush">The created brush.</param>
		/// <returns>A boolean indicating whether or not the brush was successfully created.</returns>
		public static bool TryCreateBrushFromStringRGB(string[] rgb, out SolidColorBrush brush)
		{
			if (rgb.Length == 3 && byte.TryParse(rgb[0], out var r) && byte.TryParse(rgb[1], out var g) && byte.TryParse(rgb[2], out var b))
			{
				brush = CreateBrushFromARGB(255, r, g, b);
				return true;
			}
			brush = default;
			return false;
		}
		/// <summary>
		/// Finds a brush from <see cref="Brushes"/> which has the the passed in name.
		/// Returns true if a brush is found.
		/// </summary>
		/// <param name="name">The name of the brush to search for.</param>
		/// <param name="brush">The found brush.</param>
		/// <returns>A boolean indicating whether or not a brush was successfully found.</returns>
		public static bool TryCreateBrushFromStringName(string name, out SolidColorBrush brush)
			=> _Brushes.TryGetValue(name, out brush);
		/// <summary>
		/// Creates a brush by converting <paramref name="hex"/> to <see cref="uint"/>.
		/// Returns true if a brush was successfully created.
		/// </summary>
		/// <param name="hex">The hex value in string form to create a brush from.</param>
		/// <param name="brush"></param>
		/// <returns>A boolean indicating whether or not a brush was successfully created.</returns>
		public static bool TryCreateBrushFromStringHex(string hex, out SolidColorBrush brush)
		{
			//Make sure it will always have an opacity of 255 if one isn't passed in
			var trimmed = hex.Replace("0x", "").TrimStart(new[] { '&', 'h', '#', 'x' });
			//If not 6 wide add in more 0's so the call right below doesn't mess with the colors, only the alpha channel
			while (trimmed.Length < 6)
			{
				trimmed = "0" + trimmed;
			}
			//If not 8 wide then add in more F's to make the alpha channel opaque
			while (trimmed.Length < 8)
			{
				trimmed = "F" + trimmed;
			}

			if (uint.TryParse(trimmed, NumberStyles.HexNumber, null, out var h))
			{
				brush = CreateBrushFromUInt(h);
				return true;
			}
			brush = default;
			return false;
		}
		/// <summary>
		/// Creates a brush from the uint value of a color.
		/// </summary>
		/// <param name="value">The color's value as a uint.</param>
		/// <returns>A brush with the passed in color.</returns>
		public static SolidColorBrush CreateBrushFromUInt(uint value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}

			return CreateBrushFromARGB(bytes[3], bytes[2], bytes[1], bytes[0]);
		}
		/// <summary>
		/// Creates a brush from ARGB bytes.
		/// </summary>
		/// <param name="a">The alpha channel value.</param>
		/// <param name="r">The red channel value.</param>
		/// <param name="g">The green channel value.</param>
		/// <param name="b">The blue channel value.</param>
		/// <returns>A brush with the passed in ARGB.</returns>
		public static SolidColorBrush CreateBrushFromARGB(byte a, byte r, byte g, byte b)
		{
			a = Math.Min(a, (byte)255);
			r = Math.Min(r, (byte)255);
			g = Math.Min(g, (byte)255);
			b = Math.Min(b, (byte)255);
			return new SolidColorBrush(Color.FromArgb(a, r, g, b));
		}

		/// <summary>
		/// Returns true if both brushes have the same ARGB values.
		/// </summary>
		/// <param name="b1">The first brush.</param>
		/// <param name="b2">The second brush to compare to.</param>
		/// <returns>A boolean indicating whether or not the brushes are equal.</returns>
		public static bool Equals(SolidColorBrush b1, SolidColorBrush b2)
			=> b1?.Color == b2?.Color && b1?.Opacity == b2?.Opacity;
	}
}
