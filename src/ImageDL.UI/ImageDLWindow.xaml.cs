using ImageDL.ImageDownloaders;
using ImageDL.UI.Classes;
using ImageDL.UI.Classes.Writers;
using ImageDL.UI.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageDL.UI
{
	/// <summary>
	/// Interaction logic for ImageDLWindow.xaml
	/// </summary>
	public partial class ImageDLWindow : Window, INotifyPropertyChanged
	{
		private Type _CurrentDownloaderType = typeof(IImageDownloader);
		public Type CurrentDownloaderType
		{
			get => _CurrentDownloaderType;
			private set
			{
				_CurrentDownloaderType = value;
				NotifyPropertyChanged();
			}
		}
		public Holder<IImageDownloader> Downloader { get; private set; } = new Holder<IImageDownloader>();

		public event PropertyChangedEventHandler PropertyChanged;

		public ImageDLWindow()
		{
			InitializeComponent();
			Console.SetOut(new RichTextBoxStreamWriter(Output));
		}

		private void OnOutputTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!(sender is RichTextBox rtb))
			{
				return;
			}

			rtb.ScrollToEnd();
		}
		private void OnSiteButtonClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button b) || !(b.Tag is Type t))
			{
				return;
			}
			else if (CurrentDownloaderType != t)
			{
				CurrentDownloaderType = t;
				Downloader.HeldObject = (IImageDownloader)Activator.CreateInstance(t);
			}
		}
		private void OnSetArgumentsButtonClick(object sender, RoutedEventArgs e)
		{
			//First get all the next level down arguments (most textboxes and numberboxes)
			var children = GenericArguments.GetChildren().Concat(GetArgumentGrid(CurrentDownloaderType).GetChildren());
			//Then get the arguments in viewboxes (checkboxes)
			var argChildren = children.Concat(children.OfType<Viewbox>().Select(x => x.Child));
			var tbs = argChildren.OfType<TextBox>().Where(x => x.Tag != null).Select(x => $"{x.Tag.ToString()}:{x.Text}");
			var cbs = argChildren.OfType<CheckBox>().Where(x => x.Tag != null).Select(x => $"{x.Tag.ToString()}:{x.IsChecked}");
			Downloader.HeldObject.SetArguments(tbs.Concat(cbs).ToArray());
		}
		private void OnStartDownloadsButtonClick(object sender, RoutedEventArgs e)
			=> Task.Run(async () => await Downloader.HeldObject.StartAsync());

		private Grid GetArgumentGrid(Type type)
		{
			if (type == typeof(RedditImageDownloader))
			{
				return RedditArguments;
			}
			else
			{
				throw new InvalidOperationException("This method should not be able to be reached when no settings menu is up.");
			}
		}
		private void NotifyPropertyChanged([CallerMemberName] string name = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
