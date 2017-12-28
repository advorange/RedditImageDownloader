using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageDL.Utilities;

namespace ImageDL.UI.Classes.Controls
{
	/// <summary>
	/// Interaction logic for ImageDLNumberBox.xaml
	/// </summary>
	internal partial class ImageDLNumberBox : ImageDLTextBox
	{
		private static Regex _NumberRegex = new Regex(@"[^\d-]", RegexOptions.Compiled);

		private int _DefaultValue = 0;
		public int DefaultValue
		{
			get => _DefaultValue;
			set => _DefaultValue = value;
		}
		private int _MaximumValue = int.MaxValue;
		public int MaximumValue
		{
			get => _MaximumValue;
			set
			{
				_MaximumValue = value;
				MaxLength = Math.Max(_MinimumValue.GetLengthOfNumber(), _MaximumValue.GetLengthOfNumber());
			}
		}
		private int _MinimumValue = int.MinValue;
		public int MinimumValue
		{
			get => _MinimumValue;
			set
			{
				_MinimumValue = value;
				MaxLength = Math.Max(_MinimumValue.GetLengthOfNumber(), _MaximumValue.GetLengthOfNumber());
			}
		}
		private int _StoredNum;
		public int StoredNum
		{
			get => _StoredNum;
			private set
			{
				//TODO: figure why going from default value to default value changes no text
				//Seems to be it's from going from same value to same value doesn't set text back
				if (value > MaximumValue)
				{
					_StoredNum = MaximumValue;
				}
				else if (value < MinimumValue)
				{
					_StoredNum = MinimumValue;
				}
				else
				{
					_StoredNum = value;
				}
				Text = _StoredNum.ToString();
			}
		}

		public ImageDLNumberBox()
		{
			InitializeComponent();
			DataObject.AddPastingHandler(this, OnPaste);
		}

		public override void EndInit()
		{
			base.EndInit();
			StoredNum = DefaultValue;
			MaxLength = Math.Max(_MinimumValue.GetLengthOfNumber(), _MaximumValue.GetLengthOfNumber());
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			//Update the stored value
			if (!(e.OriginalSource is TextBox tb) || String.IsNullOrWhiteSpace(tb.Text))
			{
				return;
			}
			else if (!int.TryParse(tb.Text, out var result))
			{
				StoredNum = DefaultValue;
			}
			else
			{
				StoredNum = result;
			}
		}
		private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
			//If char is null or not a number then don't let it go through
			=> e.Handled = !String.IsNullOrWhiteSpace(e.Text) && _NumberRegex.IsMatch(e.Text);
		private void OnPaste(object sender, DataObjectPastingEventArgs e)
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

		private void OnUpButtonClick(object sender, RoutedEventArgs e)
		{
			if (StoredNum >= MaximumValue)
			{
				return;
			}
			++StoredNum;
		}
		private void OnDownButtonClick(object sender, RoutedEventArgs e)
		{
			if (StoredNum <= MinimumValue)
			{
				return;
			}
			--StoredNum;
		}
	}
}