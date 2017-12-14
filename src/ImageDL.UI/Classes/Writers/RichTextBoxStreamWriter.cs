using ImageDL.UI.Utilities;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ImageDL.UI.Classes.Writers
{
	internal class RichTextBoxStreamWriter : TextWriter
    {
		private static readonly SolidColorBrush _Clicked = BrushUtils.CreateBrush("#551A8B");

		private RichTextBox _RTB;
		private string _CurrentLineText;

		public override Encoding Encoding => Encoding.UTF32;

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

			_CurrentLineText += value;
			if (value == '\n')
			{
				Write(_CurrentLineText);
				_CurrentLineText = null;
			}
		}
		public override void Write(string value)
		{
			foreach (var s in StitchFilePathsBackTogether(SplitStringByWhiteSpace(value)))
			{
				//Rich text boxes have too much space if empty lines are allowed to be printed
				if (s == "\n")
				{
					return;
				}
				//Link
				else if (UriUtils.GetIfStringIsValidUrl(s))
				{
					WriteHyperlink(CreateHyperlink(s));
				}
				//File
				else if (File.Exists(s))
				{
					WriteHyperlink(CreateHyperlink(s));
				}
				//Plain text
				else
				{
					WriteText(s);
				}
			}
		}
		private string[] SplitStringByWhiteSpace(string value)
		{
			var parts = new List<string>();
			var sb = new StringBuilder();
			foreach (var c in value)
			{
				if (Char.IsWhiteSpace(c))
				{
					parts.Add(sb.ToString());
					sb.Clear();
				}
				sb.Append(c);
			}
			if (sb.Length != 0)
			{
				parts.Add(sb.ToString());
			}
			return parts.ToArray();
		}
		private string[] StitchFilePathsBackTogether(string[] parts)
		{
			//Example arguments:
			//"c:\\dog" " pic.jpg" " lol" 
			var paths = new List<(string Path, int Start, int End)>();
			for (int i = 0; i < parts.Length; ++i)
			{
				//For this program, only allow files to be linked if they start like C:\\, D:\\ etc
				if (!parts[i].Contains(":\\"))
				{
					continue;
				}
				//"c:\\dog" doesn't exist so this won't go in here.
				//File.Exists cares if there is an extension, so "c:\\dog.jpg" wouldn't have been found either
				else if (File.Exists(parts[i]))
				{
					paths.Add((parts[i], i, i));
					continue;
				}

				//Will start with "c:\\dog", then gets the dir "c:\\"
				var currDir = parts[i].Substring(0, Math.Max(0, parts[i].LastIndexOf('\\')));
				//"c:\\" exists so keeps going. If the first directory doesn't exist then there's no point in showing any more
				if (!Directory.Exists(currDir))
				{
					continue;
				}

				//Starts searching for files or folders which have their names start with "dog"
				var searchFor = parts[i].Substring(currDir.Length + 1);
				for (int j = i + 1; j < parts.Length; ++j)
				{
					//Adds in " pic.jpg" so we're searching for "dog pic.jpg" now
					var add = parts[j];
					if (add.Contains(":"))
					{
						add = add.Substring(0, add.IndexOf(':'));
					}
					searchFor += add;

					//See if any files match the current string
					//Ends up finding "c:\\dog pic.jpg"
					var fileMatches = Directory.EnumerateFiles(currDir, $"{searchFor.TrimEnd()}*", SearchOption.TopDirectoryOnly);
					if (!fileMatches.Any())
					{
						continue;
					}

					//If there are any file matches, only look at those. If they all fail then get out of this iteration
					//Combines "c:\\" with "dog pic.jpg" to get the path of "c:\\dog pic.jpg"
					var fileTotalPath = Path.Combine(currDir, searchFor);
					for (int k = j + 1; k < parts.Length; ++k)
					{
						//First iteration checks if "c:\\dog pic.jpg" exists. It does, so returns
						if (File.Exists(fileTotalPath.TrimEnd()))
						{
							paths.Add((fileTotalPath.TrimEnd(), i, k - 1));
							i = j;
							break;
						}
						//If we were searching for "c:\\dog pic 400.jpg" it would loop around once more in the j loop and in the k loop
						fileTotalPath += parts[k];
					}
					break;
				}
			}

			//No paths were found so just return the starting array
			if (!paths.Any())
			{
				return parts;
			}

			var returnValues = new List<string>();
			for (int i = 0; i < parts.Length; ++i)
			{
				var path = paths.SingleOrDefault(x => x.Start == i);
				if (String.IsNullOrWhiteSpace(path.Path))
				{
					returnValues.Add(parts[i]);
					continue;
				}
				returnValues.AddRange(SeparateWhiteSpaceFromPath(path.Path));

				//Set the end value because all the stuff before that should be skipped since it was added to the path
				i = path.End;
			}
			return returnValues.ToArray();
		}
		private string[] SeparateWhiteSpaceFromPath(string path)
		{
			string beforeWhiteSpace = null;
			for (int j = 0; j < path.Length; ++j)
			{
				var c = path[j];
				if (!Char.IsWhiteSpace(c))
				{
					break;
				}
				beforeWhiteSpace += c;
			}
			string afterWhiteSpace = null;
			for (int j = path.Length - 1; j > 0; --j)
			{
				var c = path[j];
				if (!Char.IsWhiteSpace(c))
				{
					break;
				}
				afterWhiteSpace = c + afterWhiteSpace;
			}

			var returnValues = new List<string>();
			if (beforeWhiteSpace != null)
			{
				returnValues.Add(beforeWhiteSpace);
			}
			//Add the path in to the return values
			returnValues.Add(path.Trim());
			if (afterWhiteSpace != null)
			{
				returnValues.Add(afterWhiteSpace);
			}
			return returnValues.ToArray();
		}
		private Hyperlink CreateHyperlink(string value)
		{
			var hyperlink = new Hyperlink(new Run(value))
			{
				IsEnabled = true,
				NavigateUri = new Uri(value),
			};
			hyperlink.RequestNavigate += (sender, e) =>
			{
				((Hyperlink)sender).Foreground = _Clicked;
				try
				{
					Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
				}
				catch (Exception exc)
				{
					exc.WriteException();
				}
				e.Handled = true;
			};
			return hyperlink;
		}
		private void WriteHyperlink(Hyperlink link)
			=> _RTB.Dispatcher.InvokeAsync(() =>
			{
				if (_RTB.Document.Blocks.LastBlock is Paragraph para)
				{
					para.Inlines.Add(link);
				}
				else
				{
					_RTB.Document.Blocks.Add(new Paragraph(link) {  });
				}
			}, DispatcherPriority.ContextIdle);
		private void WriteText(string text)
			=> _RTB.Dispatcher.InvokeAsync(() =>
			{
				_RTB.AppendText(text);
			}, DispatcherPriority.ContextIdle);
	}
}
