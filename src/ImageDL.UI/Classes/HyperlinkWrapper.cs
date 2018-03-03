using ImageDL.UI.Utilities;
using ImageDL.Utilities;
using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		private const int SIZE = 64;
		private const int MAX_CACHED_IMAGES = 250 * 64 * 64 / (SIZE * SIZE);
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
			var name = Path.GetFileName(path);
			if (_Thumbnails.TryGetValue(name, out var cached))
			{
				cached.UpdateLastAccessed();
			}
			else
			{
				cached = GenerateThumbnail(path, name);
				if (cached == null)
				{
					return;
				}

				_Thumbnails.TryAdd(name, cached);
			}

			//Set the thumbnail
			Dispatcher.Invoke(() => ToolTip = new Image { Source = cached.BitmapImage, Tag = name, });
			cached.AddUsingWrapper(this);
			//If too many thumbnails are cached (default 250 thumbnails) clean the cache
			if (_Thumbnails.Keys.Count >= MAX_CACHED_IMAGES)
			{
				CleanCache();
			}
		}
		private CachedImage GenerateThumbnail(string path, string name)
		{
			try
			{
				using (var image = new MagickImage(path))
				using (var ms = new MemoryStream())
				{
					//Create a thumbnail
					image.Resize(new MagickGeometry
					{
						Width = SIZE,
						Height = SIZE,
						IgnoreAspectRatio = true,
					});
					image.Write(ms);

					//Convert the thumbnail stream to an actual image
					var bmi = new BitmapImage();
					bmi.BeginInit();
					bmi.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
					bmi.CacheOption = BitmapCacheOption.OnLoad;
					bmi.StreamSource = ms;
					bmi.UriSource = null;
					bmi.EndInit();
					bmi.Freeze();

					return new CachedImage(bmi);
				}
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
			return null;
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
			catch (InvalidOperationException)
			{
				Console.WriteLine($"This hyperlink does not have an absolute uri.");
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine($"Unable to find {e.Uri.AbsoluteUri}.");
			}
			e.Handled = true;
		}
		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			IsEnabled = (NavigateUri.IsFile && File.Exists(NavigateUri.LocalPath)) || NavigateUri.IsValidUrl();
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
			private readonly List<HyperlinkWrapper> _UsingWrappers = new List<HyperlinkWrapper>();
			public readonly BitmapImage BitmapImage;
			public long LastAccessedTicks { get; private set; }

			public CachedImage(BitmapImage bmi)
			{
				BitmapImage = bmi;
				UpdateLastAccessed();
			}
			public void AddUsingWrapper(HyperlinkWrapper wrapper)
			{
				_UsingWrappers.Add(wrapper);
			}
			public void UpdateLastAccessed()
			{
				LastAccessedTicks = DateTime.UtcNow.Ticks;
			}
			public void Clear()
			{
				foreach (var wrapper in _UsingWrappers)
				{
					wrapper.Dispatcher.Invoke(() => { wrapper.ToolTip = new TextBlock { Text = NOT_CACHED, }; });
				}
			}
		}
	}
}
