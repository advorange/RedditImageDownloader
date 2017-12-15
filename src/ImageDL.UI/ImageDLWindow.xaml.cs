using ImageDL.UI.Classes.Writers;
using System;
using System.Windows;

namespace ImageDL.UI
{
	/// <summary>
	/// Interaction logic for ImageDLWindow.xaml
	/// </summary>
	public partial class ImageDLWindow : Window
	{
		public ImageDLWindow()
		{
			InitializeComponent();
		}

		private void Output_Loaded(object sender, RoutedEventArgs e)
		{
			Output.Document.Blocks.Clear();
			Console.SetOut(new RichTextBoxStreamWriter(Output));
			//Console.WriteLine("https://www.google.com test\ntestest");
			//Console.WriteLine("test");
			//Console.WriteLine();
			//Console.WriteLine("test2");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\7jk5uh_fvepwxiwop301.jpg (Standard file)");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\D\E\E\P\7jkoo7_t2usfrtt2q301.jpg (Deep file)");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\Nonexistent File.jpg (Nonexistent file)");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\7jk5uh_fvepwxiwop30.jpg (Nonexistent, very similar file name but shorter)");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\7jk5uh_fvepwxiwop3011.jpg (Nonexistent, very similar file name but longer)");
			Console.WriteLine(@"C:\Users\Nate\Downloads\New folder (2)\7jn0ts_4llj7otxnr301.jpg C:\Users\Nate\Downloads\New folder (2)\7jjbnn_wyde7f0p2p301.jpg (Two in one)");
			Console.WriteLine(@"C:\test\7jk5uh fvepwxiwop301.jpg C:\test\7jk5uh.jpg (Names that are way too close)");

			/*
			for (int i = 0; i < 1000; ++i)
			{
				Console.WriteLine(i);
			}*/
		}
	}
}
