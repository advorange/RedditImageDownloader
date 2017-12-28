using ImageDL.Enums;
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
		private Site _CurrentSite;
		public Site CurrentSite
		{
			get => _CurrentSite;
			private set
			{
				_CurrentSite = value;
				NotifyPropertyChanged();
			}
		}
		public Holder<IImageDownloader> Downloader { get; private set; } = new Holder<IImageDownloader>();

		public event PropertyChangedEventHandler PropertyChanged;

		public ImageDLWindow()
		{
			InitializeComponent();
		}

		private void NotifyPropertyChanged([CallerMemberName] string name = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		private void OnOutputLoaded(object sender, RoutedEventArgs e)
		{
			if (!(sender is RichTextBox rtb))
			{
				return;
			}

			Console.SetOut(new RichTextBoxStreamWriter(rtb));
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
			if (!(sender is Button b) || !(b.Tag is Site s))
			{
				return;
			}
			else if (CurrentSite != s)
			{
				CurrentSite = s;
				Downloader.HeldObject = CreateDownloader();
				Downloader.HeldObject.AllArgumentsSet += OnAllArgumentsSet;
				Downloader.HeldObject.DownloadsFinished += OnDownloadsFinished;
			}
		}
		private void OnSetArgumentsButtonClick(object sender, RoutedEventArgs e)
		{
			var generalArgsTbs = GenericArguments.GetChildren().OfType<TextBox>().Where(x => x.Tag != null);
			var specificArgsTbs = GetArgumentGrid(CurrentSite).GetChildren().OfType<TextBox>().Where(x => x.Tag != null);
			var args = generalArgsTbs.Concat(specificArgsTbs).Select(x => $"{x.Tag.ToString()}:{x.Text}").ToArray();
			Downloader.HeldObject.AddArguments(args);
		}
		private Task OnAllArgumentsSet()
		{
			SetArgumentsButton.Visibility = Visibility.Collapsed;
			StartDownloadsButton.Visibility = Visibility.Visible;
			return Task.FromResult(0);
		}
		private async void OnStartDownloadsButtonClick(object sender, RoutedEventArgs e)
		{
			SiteSelector.IsEnabled = false;
			ArgumentLayout.IsEnabled = false;
			await Downloader.HeldObject.StartAsync();
		}
		private Task OnDownloadsFinished()
		{
			SiteSelector.IsEnabled = true;
			ArgumentLayout.IsEnabled = true;
			StartDownloadsButton.Visibility = Visibility.Collapsed;
			SetArgumentsButton.Visibility = Visibility.Visible;

			foreach (var tb in GenericArguments.GetChildren().OfType<TextBox>())
			{
				tb.Clear();
			}
			foreach (Site s in Enum.GetValues(typeof(Site)))
			{
				foreach (var tb in GetArgumentGrid(s).GetChildren().OfType<TextBox>())
				{
					tb.Clear();
				}
			}
			Downloader.HeldObject = null;
			CurrentSite = default;
			return Task.FromResult(0);
		}

		private Grid GetArgumentGrid(Site site)
		{
			switch (site)
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
			switch (CurrentSite)
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
