using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ImageDL.UI.Classes.Writers
{
	internal class TextBoxStreamWriter : TextWriter
	{
		private TextBox _TB;
		private string _CurrentLineText;

		public TextBoxStreamWriter(TextBox tb)
		{
			_TB = tb;
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
			if (value == null)
			{
				return;
			}

			_TB.Dispatcher.InvokeAsync(() => _TB.AppendText(value), DispatcherPriority.ContextIdle);
		}
		public override Encoding Encoding => Encoding.UTF32;
	}
}
