using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// The percentage of something.
	/// </summary>
	public struct Percentage
	{
		/// <summary>
		/// The calculated percentage.
		/// </summary>
		public readonly double Value;

		/// <summary>
		/// Creates an instance of <see cref="Percentage"/> with the value as the percentage.
		/// Values between 0 and 1 inclusive will be set regularly.
		/// Values outside of that range will be divided using some stupid one liner to make them into percentages.
		/// </summary>
		/// <param name="value">The value to convert to a percentage.</param>
		/// <param name="decimalPlaces">The amount of decimal places to have in the percentage.</param>
		public Percentage(double value, int decimalPlaces = 3)
		{
			Value = Math.Round(Math.Abs(value >= -1 && value <= 1 ? value : value / Math.Pow(10, (int)Math.Log10(Math.Abs(value)) + 1)), decimalPlaces);
		}

		/// <summary>
		/// Attempts to convert the string into a <see cref="Percentage"/>.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(string s, out Percentage result)
		{
			if (double.TryParse(s, out var d))
			{
				result = new Percentage(d);
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Value.ToString();
		}
	}
}