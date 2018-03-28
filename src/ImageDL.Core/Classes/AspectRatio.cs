using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageDL.Classes
{
	/// <summary>
	/// The ratio of width to height.
	/// </summary>
	public class AspectRatio
	{
		/// <summary>
		/// The calculated aspect ratio.
		/// </summary>
		public readonly double Value;

		/// <summary>
		/// Creates an instance of <see cref="AspectRatio"/> with the value as the 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		public AspectRatio(double v1, double v2) : this(Math.Round(v1 / v2, 3)) { }
		private AspectRatio(double value)
		{
			Value = value;
		}

		/// <summary>
		/// Attempts to convert the string into an <see cref="AspectRatio"/>.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(string s, out AspectRatio result)
		{
			if (double.TryParse(s, out var d))
			{
				result = new AspectRatio(d);
				return true;
			}

			var numStrings = new List<string>();
			var curr = new StringBuilder();
			foreach (var c in s)
			{
				//Add onto current string to increase number
				if (Char.IsNumber(c))
				{
					curr.Append(c);
				}
				else
				{
					numStrings.Add(curr.ToString());
					curr.Clear();
				}

				//Too many numbers so it's automatically invalid
				if (numStrings.Count > 2)
				{
					result = default;
					return false;
				}
			}
			if (curr.Length > 0)
			{
				numStrings.Add(curr.ToString());
			}
			//Too many numbers so it's automatically invalid
			if (numStrings.Count > 2)
			{
				result = default;
				return false;
			}

			var numbers = numStrings.Select(x => int.TryParse(x, out var i) ? i : -1).Where(x => x != -1).ToArray();
			if (numbers.Length < 2 || numbers[1] == 0)
			{
				result = default;
				return false;
			}

			result = new AspectRatio(numbers[0], numbers[1]);
			return true;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
