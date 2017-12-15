using ImageDL.UI.Utilities;
using ImageDL.Utilities;
using ImageResizer;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ImageDL.UI.Classes
{
	internal class HyperlinkWrapper : Hyperlink
	{
		private static readonly SolidColorBrush _Clicked = BrushUtils.CreateBrush("#551A8B");
		private static readonly ConcurrentDictionary<string, BitmapImage> _Thumbnails = new ConcurrentDictionary<string, BitmapImage>();

		public HyperlinkWrapper(string link) : base(new Run(link))
		{
			IsEnabled = true;
			NavigateUri = new Uri(link);

			var p = NavigateUri.LocalPath;
			if (File.Exists(p))
			{
				Task.Run(() => AddImageToThumbnails(p));
			}

			RequestNavigate += OnRequestNavigate;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;
		}

		private void AddImageToThumbnails(string path)
		{
			var name = Path.GetFileName(path);
			if (_Thumbnails.TryGetValue(name, out var bmi))
			{
				return;
			}

			using (var s = new MemoryStream())
			using (var bm = new Bitmap(path))
			{
				try
				{
					ImageBuilder.Current.Build(bm, s, new ResizeSettings(100, 100, FitMode.Stretch, null));

					bmi = new BitmapImage();
					bmi.BeginInit();
					bmi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
					bmi.CacheOption = BitmapCacheOption.OnLoad;
					bmi.StreamSource = s;
					bmi.UriSource = null;
					bmi.EndInit();
					bmi.Freeze();
				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine($"Unable to read {name}.");
				}
				catch (InvalidOperationException)
				{
					Console.WriteLine($"Unable to generate a thumbnail for {name}.");
				}
				catch (NotSupportedException)
				{
					Console.WriteLine($"Unable to generate a thumbnail for {name}.");
				}

				Dispatcher.Invoke(() => ToolTip = new System.Windows.Controls.Image() { Source = bmi, });
			}

			if (!_Thumbnails.TryAdd(name, bmi))
			{
				Console.WriteLine($"Unable to cache the thumbnail for {name}.");
			}
		}

		private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Foreground = _Clicked;
			try
			{
				Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			}
			catch (Exception exc)
			{
				exc.WriteException();
			}
			e.Handled = true;
		}
		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			IsEnabled = File.Exists(NavigateUri.LocalPath);
			if (!IsEnabled)
			{
				return;
			}

			//Display thumbnail
			if (ToolTip is ToolTip toolTip)
			{
				toolTip.IsOpen = true;
			}
		}
		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			//Hide thumbnail
			if (ToolTip is ToolTip toolTip)
			{
				toolTip.IsOpen = false;
			}
		}
	}
}
