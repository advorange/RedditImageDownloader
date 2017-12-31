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
				MaxLength = Math.Max(_MinimumValue.GetLength(), _MaximumValue.GetLength());
			}
		}
		private int _MinimumValue = int.MinValue;
		public int MinimumValue
		{
			get => _MinimumValue;
			set
			{
				_MinimumValue = value;
				MaxLength = Math.Max(_MinimumValue.GetLength(), _MaximumValue.GetLength());
			}
		}
		private int _StoredNum;
		public int StoredNum
		{
			get => _StoredNum;
			private set
			{
				//For some reason setting this value to the same value it already has doesn't update the text correctly
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
			MaxLength = Math.Max(_MinimumValue.GetLength(), _MaximumValue.GetLength());
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			//Update the stored value
			if (!(e.OriginalSource is TextBox tb) || String.IsNullOrWhiteSpace(tb.Text))
			{
				return;
			}
			StoredNum = int.TryParse(tb.Text, out var result) ? result : DefaultValue;
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
			if (StoredNum < MaximumValue)
			{
				++StoredNum;
			}
		}
		private void OnDownButtonClick(object sender, RoutedEventArgs e)
		{
			if (StoredNum > MinimumValue)
			{
				--StoredNum;
			}
		}
	}
}