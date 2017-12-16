using ImageDL.UI.Utilities;
using ImageDL.Utilities;
using ImageResizer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private static readonly ConcurrentDictionary<string, CachedImage> _Thumbnails = new ConcurrentDictionary<string, CachedImage>();
		private const int MAX_CACHED_IMAGES = 250;

		public HyperlinkWrapper(string link) : base(new Run(link))
		{
			IsEnabled = true;
			NavigateUri = new Uri(link);

			var p = NavigateUri.LocalPath;
			if (File.Exists(p))
			{
				Task.Run(async () => await AddImageToThumbnailsAsync(p));
			}

			RequestNavigate += OnRequestNavigate;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;
		}

		private async Task AddImageToThumbnailsAsync(string path)
		{
			var name = Path.GetFileName(path);
			if (_Thumbnails.TryGetValue(name, out var cached))
			{
				return;
			}
			else if (_Thumbnails.Keys.Count >= MAX_CACHED_IMAGES)
			{
				CleanCache();
			}

			await Task.Run(() =>
			{
				using (var s = new MemoryStream())
				using (var bm = new Bitmap(path))
				{
					try
					{
						ImageBuilder.Current.Build(bm, s, new ResizeSettings(100, 100, FitMode.Stretch, null));

						var bmi = new BitmapImage();
						bmi.BeginInit();
						bmi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
						bmi.CacheOption = BitmapCacheOption.OnLoad;
						bmi.StreamSource = s;
						bmi.UriSource = null;
						bmi.EndInit();
						bmi.Freeze();
						cached = new CachedImage(this, bmi);
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
				}
			});

			Dispatcher.Invoke(() => ToolTip = new System.Windows.Controls.Image() { Source = cached.BitmapImage, Tag = name, });
			if (!_Thumbnails.TryAdd(name, cached))
			{
				Console.WriteLine($"Unable to cache the thumbnail for {name}.");
			}
		}
		private void CleanCache()
		{
			var kvps = new List<KeyValuePair<string, CachedImage>>(_Thumbnails);
			foreach (var kvp in kvps.OrderBy(x => x.Value.LastAccessedTicks).Take(MAX_CACHED_IMAGES / 10))
			{
				//Remove it from the cache
				_Thumbnails.TryRemove(kvp.Key, out var cached);
				//Remove it from the tooltip
				kvp.Value.Clear();
			}
		}
		//TODO: allow images to be recached if need be

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
			IsEnabled = (NavigateUri.IsFile && File.Exists(NavigateUri.LocalPath)) || UriUtils.GetIfStringIsValidUrl(NavigateUri.AbsoluteUri);
			if (!IsEnabled || !(ToolTip is ToolTip toolTip))
			{
				return;
			}

			//Show thumbnail
			toolTip.IsOpen = true;
			if (_Thumbnails.TryGetValue(toolTip.Tag.ToString(), out var cached))
			{
				cached.UpdateLastAccessed();
			}
		}
		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			if (!(ToolTip is ToolTip toolTip))
			{
				return;
			}

			//Hide thumbnail
			toolTip.IsOpen = false;
		}

		private class CachedImage
		{
			private readonly HyperlinkWrapper _Wrapper;
			public readonly BitmapImage BitmapImage;
			public long LastAccessedTicks { get; private set; }

			public CachedImage(HyperlinkWrapper wrapper, BitmapImage bmi)
			{
				_Wrapper = wrapper;
				BitmapImage = bmi;
				UpdateLastAccessed();
			}

			public void UpdateLastAccessed() => LastAccessedTicks = DateTime.UtcNow.Ticks;
			public void Clear() => _Wrapper.ToolTip = null;
		}
	}
}
