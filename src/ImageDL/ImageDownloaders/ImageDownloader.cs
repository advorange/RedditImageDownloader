using ImageDL.Classes;
using ImageDL.Utilities;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Non generic abstraction of <see cref="GenericImageDownloader{TPost}"/>.
	/// </summary>
	public abstract class ImageDownloader : INotifyPropertyChanged
	{
		public OptionSet CommandLineParserOptions;

		private string _Directory;
		private int _AmountToDownload;
		private int _MinWidth;
		private int _MinHeight;
		private int _MaxDaysOld;
		private int _MaxImageSimilarity;
		private int _ImagesCachedPerThread;
		private bool _CompareSavedImages;
		private bool _AllArgumentsSet;
		private bool _BusyDownloading;
		private bool _DownloadsFinished;

		public string Directory
		{
			get => _Directory;
			set
			{
				if (!String.IsNullOrWhiteSpace(value) && !System.IO.Directory.Exists(value))
				{
					Console.WriteLine($"{Directory} does not exist as a directory.");
					_SetArguments.RemoveAll(x => x.Name == nameof(Directory));
					return;
				}

				_Directory = value;
				NotifyPropertyChanged();
			}
		}
		public int AmountToDownload
		{
			get => Math.Max(1, _AmountToDownload);
			set
			{
				_AmountToDownload = value;
				NotifyPropertyChanged();
			}
		}
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = value;
				NotifyPropertyChanged();
			}
		}
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = value;
				NotifyPropertyChanged();
			}
		}
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = value;
				NotifyPropertyChanged();
			}
		}
		public int MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = Math.Min(1000, Math.Max(1, value));
				NotifyPropertyChanged();
			}
		}
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set
			{
				_ImagesCachedPerThread = value;
				NotifyPropertyChanged();
			}
		}
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set
			{
				_CompareSavedImages = value;
				NotifyPropertyChanged();
			}
		}
		/// <summary>
		/// Returns true if all arguments (aside from ones with default values) have been set at least once.
		/// </summary>
		public bool AllArgumentsSet
		{
			get => _AllArgumentsSet;
			protected set
			{
				_AllArgumentsSet = value;
				NotifyPropertyChanged();
			}
		}
		/// <summary>
		/// Returns true when images are being downloaded.
		/// </summary>
		public bool BusyDownloading
		{
			get => _BusyDownloading;
			protected set
			{
				_BusyDownloading = value;
				NotifyPropertyChanged();
			}
		}
		/// <summary>
		/// Returns true after all images have been downloaded.
		/// </summary>
		public bool DownloadsFinished
		{
			get => _DownloadsFinished;
			protected set
			{
				_DownloadsFinished = value;
				NotifyPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected ImmutableDictionary<string, PropertyInfo> _Arguments;
		protected List<PropertyInfo> _SetArguments = new List<PropertyInfo>();
		protected List<ContentLink> _AnimatedContent = new List<ContentLink>();
		protected List<ContentLink> _FailedDownloads = new List<ContentLink>();
		protected ImageComparer _ImageComparer;

		public ImageDownloader()
		{
			CommandLineParserOptions = new OptionSet()
				.Add($"d|dir|{nameof(Directory)}=", "the directory to save to.", x => Directory = x)
				.Add($"a|amt|{nameof(AmountToDownload)}=", "the amount of images to download.", x => AmountToDownload = Convert.ToInt32(x))
				.Add($"mw|minw|{nameof(MinWidth)}=", "the minimum width to save an image with.", x => MinWidth = Convert.ToInt32(x))
				.Add($"mh|minh|{nameof(MinHeight)}=", "the minimum height to save an image with.", x => MinHeight = Convert.ToInt32(x))
				.Add($"da|days|{nameof(MaxDaysOld)}=", "the oldest an image can be before it won't be saved.", x => MaxDaysOld = Convert.ToInt32(x))
				.Add($"s|sim|{nameof(MaxImageSimilarity)}=", "the percentage similarity before an image should be deleted (1 = .1%, 1000 = 100%).", x => MaxImageSimilarity = Convert.ToInt32(x))
				.Add($"c|cached|{nameof(ImagesCachedPerThread)}=", "how many images to cache on each thread (lower = faster but more CPU).", x => ImagesCachedPerThread = Convert.ToInt32(x))
				.Add($"csi|compare|{nameof(CompareSavedImages)}=", "whether or not to compare to already saved images.", x => CompareSavedImages = Convert.ToBoolean(x));

			_Arguments = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.GetSetMethod() != null && x.GetGetMethod() != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToImmutableDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

			MaxImageSimilarity = 1000;
			ImagesCachedPerThread = 50;
			CompareSavedImages = false;

			//Save on close in case program is closed while running
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => SaveStoredContentLinks();
		}

		/// <summary>
		/// Start downloading images.
		/// </summary>
		/// <returns>An asynchronous task which downloads images.</returns>
		public abstract Task StartAsync();
		/// <summary>
		/// Sets arguments with the supplied array of data.
		/// </summary>
		public void SetArguments(string[] args)
		{
			try
			{
				var extra = CommandLineParserOptions.Parse(args);
				if (extra.Any())
				{
					Console.WriteLine($"The following parts were extra, was an argument mistyped? '{String.Join("', '", extra)}'");
				}
			}
			catch (FormatException fe)
			{
				fe.Write();
			}
			catch (OptionException oe)
			{
				oe.Write();
			}
		}
		/// <summary>
		/// Prints out to the console what arguments are still needed.
		/// </summary>
		public void AskForArguments()
		{
			var unsetArgs = _Arguments.Where(x => !_SetArguments.Contains(x.Value));
			if (!unsetArgs.Any())
			{
				return;
			}

			var sb = new StringBuilder("The following arguments need to be set:" + Environment.NewLine);
			foreach (var kvp in unsetArgs)
			{
				sb.AppendLine($"\t{kvp.Key} ({kvp.Value.PropertyType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}
		/// <summary>
		/// Saves the stored content links to file.
		/// </summary>
		public void SaveStoredContentLinks()
		{
			if (String.IsNullOrWhiteSpace(Directory))
			{
				return;
			}

			SaveContentLinks(ref _AnimatedContent, new FileInfo(Path.Combine(Directory, "Animated_Content.txt")));
			SaveContentLinks(ref _FailedDownloads, new FileInfo(Path.Combine(Directory, "Failed_Downloads.txt")));
		}
		/// <summary>
		/// Invokes <see cref="PropertyChanged"/>.
		/// </summary>
		/// <param name="name">The property changed.</param>
		protected void NotifyPropertyChanged([CallerMemberName] string name = "")
		{
			if (_Arguments.TryGetValue(name, out var property) && !_SetArguments.Any(x => x.Name == name))
			{
				Console.WriteLine($"Successfully set {name}.");
				_SetArguments.Add(property);
			}
			if (!AllArgumentsSet && !_Arguments.Any(x => !_SetArguments.Contains(x.Value)))
			{
				AllArgumentsSet = true;
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private void SaveContentLinks(ref List<ContentLink> contentLinks, FileInfo file)
		{
			//Only bother saving if any exist
			if (!contentLinks.Any())
			{
				return;
			}

			//Only save links which are not already in the text document
			var unsavedContent = new List<ContentLink>();
			if (file.Exists)
			{
				using (var reader = new StreamReader(file.OpenRead()))
				{
					var text = reader.ReadToEnd();
					foreach (var anim in contentLinks)
					{
						if (text.Contains(anim.Uri.ToString()))
						{
							continue;
						}

						unsavedContent.Add(anim);
					}
				}
			}
			else
			{
				unsavedContent = _AnimatedContent;
			}

			if (!unsavedContent.Any())
			{
				return;
			}

			//Save all the links then say how many were saved
			var title = Path.GetFileName(file.FullName).Replace("_", " ").FormatTitle();
			var len = unsavedContent.Max(x => x.Score).ToString().Length;
			var format = unsavedContent.OrderByDescending(x => x.Score).Select(x => $"{x.Score.ToString().PadLeft(len, '0')} {x.Uri}");
			var write = String.Join(Environment.NewLine, format);
			using (var writer = file.AppendText())
			{
				writer.WriteLine($"{title} - {Utils.FormatDateTimeForSaving()}");
				writer.WriteLine(write);
				writer.WriteLine();
			}
			Console.WriteLine($"Added {unsavedContent.Count()} links to {file.Name}.");
			contentLinks.Clear();
		}
	}
}
