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

		public static SolidColorBrush CreateBrush(string input)
		{
			var split = input.Split('/');
			if (split.Length == 3 && TryCreateBrushFromStringRGB(split, out var rgb))
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
		public static bool TryCreateBrush(string input, out SolidColorBrush brush)
		{
			var split = input.Split('/');
			if (split.Length == 3 && TryCreateBrushFromStringRGB(split, out var rgb))
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
		private static bool TryCreateBrushFromStringRGB(string[] rgb, out SolidColorBrush color)
		{
			if (rgb.Length == 3 && byte.TryParse(rgb[0], out var r) && byte.TryParse(rgb[1], out var g) && byte.TryParse(rgb[2], out var b))
			{
				color = CreateBrushFromARGB(255, r, g, b);
				return true;
			}
			color = default;
			return false;
		}
		private static bool TryCreateBrushFromStringName(string name, out SolidColorBrush color) => _Brushes.TryGetValue(name, out color);
		private static bool TryCreateBrushFromStringHex(string hex, out SolidColorBrush color)
		{
			//Make sure it will always have an opacity of 255 if one isn't passed in
			var trimmed = hex.TrimStart(new[] { '&', 'h', '#', 'x' });
			while (trimmed.Length < 8)
			{
				trimmed = "F" + trimmed;
			}

			if (uint.TryParse(trimmed, NumberStyles.HexNumber, null, out uint h))
			{
				color = CreateBrushFromInt(h);
				return true;
			}
			color = default;
			return false;
		}
		private static SolidColorBrush CreateBrushFromInt(uint value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}

			return CreateBrushFromARGB(bytes[3], bytes[0], bytes[1], bytes[2]);
		}
		private static SolidColorBrush CreateBrushFromARGB(byte a, byte r, byte g, byte b)
		{
			a = Math.Min(a, (byte)255);
			r = Math.Min(r, (byte)255);
			g = Math.Min(g, (byte)255);
			b = Math.Min(b, (byte)255);
			return new SolidColorBrush(Color.FromArgb(a, r, g, b));
		}

		public static bool CheckIfSameBrush(SolidColorBrush b1, SolidColorBrush b2) => b1?.Color == b2?.Color && b1?.Opacity == b2?.Opacity;
	}
}
