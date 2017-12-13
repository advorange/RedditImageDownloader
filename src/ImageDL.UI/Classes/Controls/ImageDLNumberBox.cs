using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// A <see cref="ImageDLTextBox"/> which only accepts numbers as input.
	/// </summary>
	internal class ImageDLNumberBox : ImageDLTextBox
	{
		private static Regex _NumberRegex = new Regex(@"[^\d]", RegexOptions.Compiled);

		public ImageDLNumberBox()
		{
			PreviewTextInput += Validate;
			DataObject.AddPastingHandler(this, Validate);
		}

		private void Validate(object sender, TextCompositionEventArgs e)
			=> e.Handled = !String.IsNullOrWhiteSpace(e.Text) && !char.IsDigit(e.Text, Math.Min(0, e.Text.Length - 1));
		private void Validate(object sender, DataObjectPastingEventArgs e)
		{
			if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true) || !(e.Source is TextBox tb))
			{
				return;
			}

			var input = e.SourceDataObject.GetData(DataFormats.UnicodeText).ToString();
			var nums = _NumberRegex.Replace(input, "");

			//Append the text in the correct part of the string
			var sb = new StringBuilder();
			for (int i = 0; i < tb.MaxLength; ++i)
			{
				if (i < tb.CaretIndex)
				{
					sb.Append(tb.Text[i]);
				}
				else if (i < tb.CaretIndex + nums.Length)
				{
					sb.Append(nums[i - tb.CaretIndex]);
				}
				else if (i < tb.Text.Length + nums.Length)
				{
					sb.Append(tb.Text[i - nums.Length]);
				}
			}
			tb.Text = sb.ToString();
			tb.CaretIndex = tb.Text.Length;

			e.CancelCommand();
		}
	}
}
