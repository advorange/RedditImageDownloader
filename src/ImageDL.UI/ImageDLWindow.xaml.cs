using ImageDL.UI.Classes.Writers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

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
			Console.WriteLine("https://www.google.com");
			Console.WriteLine("test");
			Console.WriteLine("test2");
		}
	}
}
