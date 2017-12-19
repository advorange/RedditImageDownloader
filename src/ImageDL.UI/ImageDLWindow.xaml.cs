using ImageDL.Enums;
using ImageDL.ImageDownloaders;
using ImageDL.UI.Classes.Writers;
using ImageDL.UI.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI
{
	/// <summary>
	/// Interaction logic for ImageDLWindow.xaml
	/// </summary>
	public partial class ImageDLWindow : Window
	{
		private Site _CurrentSite;
		private IImageDownloader _Downloader;

		public ImageDLWindow()
		{
			InitializeComponent();
		}

		private void OnOutputLoaded(object sender, RoutedEventArgs e)
		{
			Output.Document.Blocks.Clear();
			Console.SetOut(new RichTextBoxStreamWriter(Output));


#if false   //Tests for displaying files
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT6.jpg (Standard file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\D\E\E\P\7jwvr4_l238w9dw20401.jpg (Deep file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\Nonexistent File.jpg (Nonexistent file)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT.jpg (Nonexistent, very similar file name but shorter)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jyouq_ZUMJiT66.jpg (Nonexistent, very similar file name but longer)");
			Console.WriteLine(@"C:\Users\User\Downloads\New folder (2)\7jz23y_9v54sn0qj2401.jpg C:\Users\User\Downloads\New folder (2)\7jzv94_e6eogbqse3401.jpg (Two in one)");
			Console.WriteLine(@"C:\test\7k0zcr gztc0njna4401.jpg C:\test\7k0zcr.jpg (Names that are way too close)");
#endif
		}
		private void OnSiteButtonClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button b) || !(b.Tag is Site s))
			{
				return;
			}

			switch (s)
			{
				case Site.Reddit:
				{
					RedditArguments.Visibility = Visibility.Visible;
					break;
				}
			}

			if (_CurrentSite != s)
			{
				_CurrentSite = s;
				_Downloader = CreateDownloader();
			}
			SetArgumentsButton.IsEnabled = true;
		}
		private void OnSetArgumentsButtonClick(object sender, RoutedEventArgs e)
		{
			if (_Downloader != null)
			{
				var a = 1;
			}
			var generalArgsTbs = GenericArguments.GetChildren().OfType<TextBox>().Where(x => !String.IsNullOrWhiteSpace(x.Tag.ToString()));
			var specificArgsTbs = GetCurrentlyUpGrid().GetChildren().OfType<TextBox>().Where(x => !String.IsNullOrWhiteSpace(x.Tag.ToString()));
			var args = generalArgsTbs.Concat(specificArgsTbs).Select(x => $"{x.Tag.ToString()}:{x.Text ?? ""}").ToArray();

			_Downloader.SetArguments(args);
			if (_Downloader.IsReady)
			{
				_Downloader = CreateDownloader();
				SetArgumentsButton.Content = "Start Downloading";
			}
		}

		private Grid GetCurrentlyUpGrid()
		{
			switch (_CurrentSite)
			{
				case Site.Reddit:
				{
					return RedditArguments;
				}
				default:
				{
					throw new InvalidOperationException("This method should not be able to be reached when no settings menu is up.");
				}
			}
		}
		private IImageDownloader CreateDownloader()
		{
			switch (_CurrentSite)
			{
				case Site.Reddit:
				{
					return new RedditImageDownloader();
				}
				default:
				{
					throw new InvalidOperationException("This method should not be able to be reached when no settings menu is up.");
				}
			}
		}
	}
}
