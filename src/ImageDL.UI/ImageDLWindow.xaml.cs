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
			Console.WriteLine("https://www.google.com test\ntestest");
			//Console.WriteLine("test");
			//Console.WriteLine();
			//Console.WriteLine("test2");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT6.jpg (Standard file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\D\E\E\P\7jwvr4_l238w9dw20401.jpg (Deep file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\Nonexistent File.jpg (Nonexistent file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT.jpg (Nonexistent, very similar file name but shorter)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT66.jpg (Nonexistent, very similar file name but longer)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jz23y_9v54sn0qj2401.jpg C:\Users\User\Downloads\New folder (2)\7jzv94_e6eogbqse3401.jpg (Two in one)");
			Console.WriteLine(@"C:\test\7k0zcr gztc0njna4401.jpg C:\test\7k0zcr.jpg (Names that are way too close)");

			/*
			for (int i = 0; i < 1000; ++i)
			{
				Console.WriteLine(i);
			}*/
		}
	}
}
