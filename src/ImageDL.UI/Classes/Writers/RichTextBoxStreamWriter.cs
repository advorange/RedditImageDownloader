using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ImageDL.UI.Classes.Writers
{
	internal class RichTextBoxStreamWriter : TextWriter
	{
		private RichTextBox _RTB;
		private string _CurrentLineText;
		private DispatcherPriority _Priority = DispatcherPriority.ContextIdle;

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
			if (_CurrentLineText.EndsWith(Environment.NewLine))
			{
				Write(_CurrentLineText);
				_CurrentLineText = null;
			}
		}
		public override void Write(string value)
		{
			foreach (var t in JoinEverythingBackTogether(StitchFilePathsBackTogether(SplitStringByWhiteSpace(value))))
			{
				//Rich text boxes have too much space if empty lines are allowed to be printed
				if (t.Text == "\n")
				{
					return;
				}
				//File or link
				else if (t.IsUri)
				{
					WriteHyperlinkAsync(new HyperlinkWrapper(t.Text));
				}
				//Plain text
				else
				{
					WriteTextAsync(t.Text);
				}
			}
		}
		private string[] SplitStringByWhiteSpace(string value)
		{
			var parts = new List<string>();
			var sb = new StringBuilder();
			foreach (var c in value)
			{
				sb.Append(c);
				if (Char.IsWhiteSpace(c))
				{
					parts.Add(sb.ToString());
					sb.Clear();
				}
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
			var paths = new List<FilePath>();
			for (int i = 0; i < parts.Length; ++i)
			{
				//For this program, only allow files to be linked if they start like C:\\, D:\\ etc
				if (!Environment.GetLogicalDrives().Any(x => parts[i].Contains(x)))
				{
					continue;
				}
				//Don't bother checking the string if it already has an invalid name
				else if (Path.GetInvalidPathChars().Any(x => parts[i].Contains(x)))
				{
					continue;
				}
				//"c:\\dog" doesn't exist so this won't go in here.
				//File.Exists cares if there is an extension, so "c:\\dog.jpg" wouldn't have been found either
				var firstFileCheck = new FileInfo(parts[i]);
				if (firstFileCheck.Exists)
				{
					paths.Add(new FilePath(firstFileCheck, i, i));
					continue;
				}
				//Will start with "c:\\dog", then gets the dir "c:\\"
				//Can't just use Path.GetDirectoryName because that leaves out the whitespace
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
					var add = new StringBuilder();
					foreach (var c in parts[j])
					{
						if (Path.GetInvalidPathChars().Contains(c))
						{
							//Breaks out of j loop
							break;
						}
						add.Append(c);
					}
					searchFor += add.ToString();

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
						var secondFileCheck = new FileInfo(fileTotalPath.TrimEnd());
						if (secondFileCheck.Exists)
						{
							paths.Add(new FilePath(secondFileCheck, i, k - 1));
							i = j;
							//Breaks out of k loop
							break;
						}
						//If we were searching for "c:\\dog pic 400.jpg" it would loop around once more in the j loop and in the k loop
						fileTotalPath += parts[k];
					}
					//Breaks out of j loop
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
				returnValues.AddRange(SeparateExtraTextFromPath(path.FullText, path.Path));

				//Set the end value because all the stuff before that should be skipped since it was added to the path
				i = path.End;
			}
			return returnValues.ToArray();
		}
		private string[] SeparateExtraTextFromPath(string text, string path)
		{
			var splitByPath = text.Split(new[] { path }, StringSplitOptions.None);
			var returnValues = new List<string>();
			if (splitByPath.Length > 0)
			{
				returnValues.Add(splitByPath[0]);
			}
			//Add the path in to the return values
			returnValues.Add(path.Trim());
			if (splitByPath.Length > 1)
			{
				returnValues.Add(splitByPath[1]);
			}
			return returnValues.ToArray();
		}
		private OutputText[] JoinEverythingBackTogether(string[] parts)
		{
			var returnValues = new List<OutputText>();
			var currentPart = new StringBuilder();
			foreach (var part in parts)
			{
				if (!File.Exists(part) && !Utils.GetIfStringIsValidUrl(part))
				{
					currentPart.Append(part);
					continue;
				}

				if (currentPart.Length != 0)
				{
					returnValues.Add(new OutputText(currentPart.ToString(), false));
					currentPart.Clear();
				}
				returnValues.Add(new OutputText(part, true));
			}
			if (currentPart.Length != 0)
			{
				returnValues.Add(new OutputText(currentPart.ToString(), false));
			}
			return returnValues.ToArray();
		}
		private async void WriteHyperlinkAsync(Hyperlink link)
			=> await _RTB.Dispatcher.InvokeAsync(() =>
			{
				if (_RTB.Document.Blocks.LastBlock is Paragraph para)
				{
					para.Inlines.Add(link);
				}
				else
				{
					_RTB.Document.Blocks.Add(new Paragraph(link));
				}
			}, _Priority);
		private async void WriteTextAsync(string text)
			=> await _RTB.Dispatcher.InvokeAsync(() =>
			{
				_RTB.AppendText(text);
			}, _Priority);

		private struct FilePath
		{
			public readonly string FullText;
			public readonly string Path;
			public readonly int Start;
			public readonly int End;

			public FilePath(FileInfo fileInfo, int start, int end)
			{
				FullText = fileInfo.ToString();
				Path = fileInfo.FullName;
				Start = start;
				End = end;
			}
		}
		private struct OutputText
		{
			public readonly string Text;
			public readonly bool IsUri;

			public OutputText(string text, bool isUri)
			{
				Text = text;
				IsUri = isUri;
			}
		}
	}
}
