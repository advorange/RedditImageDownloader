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

		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(int), typeof(ImageDLNumberBox), new PropertyMetadata(0));
		public int DefaultValue
		{
			get => (int)GetValue(DefaultValueProperty);
			set => SetValue(DefaultValueProperty, value);
		}
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(ImageDLNumberBox), new PropertyMetadata(int.MaxValue, UpdateMaxLength));
		public int MaxValue
		{
			get => (int)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(ImageDLNumberBox), new PropertyMetadata(int.MinValue, UpdateMaxLength));
		public int MinValue
		{
			get => (int)GetValue(MinValueProperty);
			set => SetValue(MinValueProperty, value);
		}
		private static readonly DependencyPropertyKey StoredValuePropertyKey = DependencyProperty.RegisterReadOnly("StoredValue", typeof(int), typeof(ImageDLNumberBox), new PropertyMetadata(0, UpdateText));
		public static readonly DependencyProperty StoredValueProperty = StoredValuePropertyKey.DependencyProperty;
		public int StoredValue
		{
			get => (int)GetValue(StoredValueProperty);
			private set => SetValue(StoredValuePropertyKey, value);
		}

		public ImageDLNumberBox()
		{
			InitializeComponent();
			DataObject.AddPastingHandler(this, OnPaste);
		}

		public override void EndInit()
		{
			base.EndInit();
			Text = DefaultValue.ToString();
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			//Update the stored value
			if (!(e.OriginalSource is TextBox tb) || String.IsNullOrWhiteSpace(tb.Text))
			{
				return;
			}
			UpdateStoredValue(int.TryParse(tb.Text, out var result) ? result : DefaultValue);
		}
		private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !String.IsNullOrWhiteSpace(e.Text) && _NumberRegex.IsMatch(e.Text);
		}

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
			if (StoredValue < MaxValue)
			{
				UpdateStoredValue(StoredValue + 1);
			}
		}
		private void OnDownButtonClick(object sender, RoutedEventArgs e)
		{
			if (StoredValue > MinValue)
			{
				UpdateStoredValue(StoredValue - 1);
			}
		}
		private void UpdateStoredValue(int value)
		{
			if (value > MaxValue)
			{
				value = MaxValue;
			}
			else if (value < MinValue)
			{
				value = MinValue;
			}

			if (StoredValue == value)
			{
				return;
			}
			else
			{
				StoredValue = value;
			}
		}

		private static void UpdateMaxLength(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var nb = d as ImageDLNumberBox;
			nb.MaxLength = Math.Max(nb.MinValue.ToString().Length, nb.MaxValue.ToString().Length);
		}
		private static void UpdateText(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var nb = d as ImageDLNumberBox;
			nb.Text = e.NewValue.ToString();
		}
	}
}