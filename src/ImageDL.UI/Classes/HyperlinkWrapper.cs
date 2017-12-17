using ImageDL.UI.Utilities;
using ImageDL.Utilities;
using ImageResizer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		private const int SQUARE_SIZE = 100;
		private const int MAX_CACHED_IMAGES = 3 * 100 * 100 / (SQUARE_SIZE * SQUARE_SIZE);
		private const string NOT_CACHED = "No thumbnail is currently available.";

		public HyperlinkWrapper(string path) : base(new Run(path))
		{
			IsEnabled = true;
			NavigateUri = new Uri(path);

			var p = NavigateUri.LocalPath;
			if (File.Exists(p))
			{
				Task.Run(() => AddImageToThumbnails(p));
			}

			RequestNavigate += OnRequestNavigate;
			MouseEnter += OnMouseEnter;
		}

		private void AddImageToThumbnails(string path)
		{
			var ticks = DateTime.UtcNow.Ticks;
			var name = Path.GetFileName(path);
			if (_Thumbnails.TryGetValue(name, out var cached))
			{
				Dispatcher.Invoke(() => ToolTip = new Image() { Source = cached.BitmapImage, Tag = name, });
				cached.UpdateLastAccessed();
				return;
			}

			using (var s = new MemoryStream())
			using (var bm = new System.Drawing.Bitmap(path))
			{
				try
				{
					//Create a thumbnail
					ImageBuilder.Current.Build(bm, s, new ResizeSettings(SQUARE_SIZE, SQUARE_SIZE, FitMode.Stretch, null));

					//Convert the thumbnail stream to an actual image
					var bmi = new BitmapImage();
					bmi.BeginInit();
					bmi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
					bmi.CacheOption = BitmapCacheOption.OnLoad;
					bmi.StreamSource = s;
					bmi.UriSource = null;
					bmi.EndInit();
					bmi.Freeze();
					cached = new CachedImage(this, bmi, ticks);
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

			//Set the thumbnail
			Dispatcher.Invoke(() => ToolTip = new Image() { Source = cached.BitmapImage, Tag = name, });
			//Cache the thumbnail
			if (!_Thumbnails.TryAdd(name, cached))
			{
				Console.WriteLine($"Unable to cache the thumbnail for {name}.");
			}
			//If too many thumbnails are cached (default 250 thumbnails) clean the cache
			else if (_Thumbnails.Keys.Count >= MAX_CACHED_IMAGES)
			{
				CleanCache();
			}
		}
		private void CleanCache()
		{
			var kvps = new List<KeyValuePair<string, CachedImage>>(_Thumbnails);
			foreach (var kvp in kvps.OrderBy(x => x.Value.LastAccessedTicks).Take(Math.Max(MAX_CACHED_IMAGES / 10, 1)))
			{
				//Remove it from the cache
				_Thumbnails.TryRemove(kvp.Key, out var cached);
				//Remove it from the tooltip
				kvp.Value.Clear();
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
			IsEnabled = (NavigateUri.IsFile && File.Exists(NavigateUri.LocalPath)) || UriUtils.GetIfUriIsValidUrl(NavigateUri);
			if (!IsEnabled)
			{
				return;
			}
			else if (ToolTip is Image img && _Thumbnails.TryGetValue(img.Tag.ToString(), out var cached))
			{
				cached.UpdateLastAccessed();
			}
			else if (ToolTip is TextBlock tb && tb.Text == NOT_CACHED)
			{
				//The path variable has to be gotten before Task.Run
				//Otherwise the UI thread doesn't let it be gotten
				var p = NavigateUri.LocalPath;
				Task.Run(() => AddImageToThumbnails(p));
			}
		}

		private class CachedImage
		{
			private readonly HyperlinkWrapper _Wrapper;
			public readonly BitmapImage BitmapImage;
			public long LastAccessedTicks { get; private set; }

			public CachedImage(HyperlinkWrapper wrapper, BitmapImage bmi, long ticks)
			{
				var test = Math.Max(bmi.PixelWidth, bmi.PixelHeight);
				_Wrapper = wrapper;
				BitmapImage = bmi;
				LastAccessedTicks = ticks;
			}
			public CachedImage(HyperlinkWrapper wrapper, BitmapImage bmi)
			{
				_Wrapper = wrapper;
				BitmapImage = bmi;
				UpdateLastAccessed();
			}

			public void UpdateLastAccessed() => LastAccessedTicks = DateTime.UtcNow.Ticks;
			public void Clear() => _Wrapper.Dispatcher.Invoke(() =>
			{
				_Wrapper.ToolTip = new TextBlock
				{
					Text = NOT_CACHED,
				};
			});
		}
	}
}
