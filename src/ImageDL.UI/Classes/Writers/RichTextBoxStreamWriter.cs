using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ImageDL.UI.Classes.Writers
{
    internal class RichTextBoxStreamWriter : TextWriter
    {
		private RichTextBox _RTB;
		private string _CurrentLineText;

		private const string HTTPS = "https://";
		private static readonly SolidColorBrush _Clicked = (SolidColorBrush)(new BrushConverter().ConvertFrom("#551A8B"));

		public RichTextBoxStreamWriter(RichTextBox rtb)
		{
			_RTB = rtb;
		}

		public override void Write(char value)
		{
			//Done because crashes program without exception.
			//Could not for the life of me figure out why.
			if (value.Equals('﷽'))
			{
				return;
			}
			else if (value.Equals('\n'))
			{
				Write(_CurrentLineText);
				_CurrentLineText = null;
			}

			_CurrentLineText += value;
		}
		public override void Write(string value)
		{
			//Rich text boxes have too much space if empty lines are allowed to be printed
			value = value.Replace("\r", "");
			if (value == null || value.Equals('\n'))
			{
				return;
			}
			//Link
			else if (value.Contains(HTTPS))
			{
				//var parts = new List<string>();
				//var urls = new List<>
				//var split = value.Split(new[] { HTTPS }, StringSplitOptions.None);

				var hyperlink = new Hyperlink(new Run(value))
				{
					IsEnabled = true,
					NavigateUri = new Uri(value),
				};
				hyperlink.RequestNavigate += (sender, e) =>
				{
					((Hyperlink)sender).Foreground = _Clicked;
					Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
					e.Handled = true;
				};
				var paragraph = new Paragraph(hyperlink);
				_RTB.Dispatcher.InvokeAsync(() => _RTB.Document.Blocks.Add(paragraph), DispatcherPriority.ContextIdle);
			}
			//Plain text
			else
			{
				_RTB.Dispatcher.InvokeAsync(() => _RTB.AppendText(value), DispatcherPriority.ContextIdle);
			}
		}
		public override Encoding Encoding => Encoding.UTF32;
	}
}
