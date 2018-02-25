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
		/// <summary>
		/// The directory to save images to.
		/// </summary>
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
				NotifyPropertyChanged(_Directory);
			}
		}
		/// <summary>
		/// The amount of posts to look through.
		/// </summary>
		public int AmountToDownload
		{
			get => _AmountToDownload;
			set
			{
				_AmountToDownload = Math.Max(1, value);
				NotifyPropertyChanged(_AmountToDownload);
			}
		}
		/// <summary>
		/// The minimum width an image can have before it won't be downloaded.
		/// </summary>
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = Math.Max(0, value);
				NotifyPropertyChanged(_MinWidth);
			}
		}
		/// <summary>
		/// The minimum height an image can have before it won't be downloaded.
		/// </summary>
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = Math.Max(0, value);
				NotifyPropertyChanged(_MinHeight);
			}
		}
		/// <summary>
		/// The maximum age an image can have before it won't be downloaded.
		/// </summary>
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = Math.Max(0, value);
				NotifyPropertyChanged(_MaxDaysOld);
			}
		}
		/// <summary>
		/// The maximum allowed image similarity before an image is considered a duplicate.
		/// </summary>
		public int MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = Math.Min(1000, Math.Max(1, value));
				NotifyPropertyChanged(_MaxImageSimilarity);
			}
		}
		/// <summary>
		/// How many images to cache per thread. Lower = faster, but more CPU.
		/// </summary>
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set
			{
				_ImagesCachedPerThread = Math.Max(1, value);
				NotifyPropertyChanged(_ImagesCachedPerThread);
			}
		}
		/// <summary>
		/// Indicates whether or not to add already saved images to the cache before downloading images.
		/// </summary>
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set
			{
				_CompareSavedImages = value;
				NotifyPropertyChanged(_CompareSavedImages);
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
				NotifyPropertyChanged(_AllArgumentsSet);
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
				NotifyPropertyChanged(_BusyDownloading);
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
				NotifyPropertyChanged(_DownloadsFinished);
			}
		}
		/// <summary>
		/// The options for setting settings from the command line.
		/// </summary>
		public OptionSet CommandLineParserOptions;
		/// <summary>
		/// Indicates when a setting has been set.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		protected ImmutableDictionary<string, PropertyInfo> _Arguments;
		protected List<PropertyInfo> _SetArguments = new List<PropertyInfo>();
		protected List<ContentLink> _AnimatedContent = new List<ContentLink>();
		protected List<ContentLink> _FailedDownloads = new List<ContentLink>();
		protected ImageComparer _ImageComparer;

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

		public ImageDownloader()
		{
			CommandLineParserOptions = new OptionSet()
			{
				{
					"h|help=",
					"help command.",
					DisplayHelp
				},
				{
					$"d|dir|{nameof(Directory)}=",
					"the directory to save to.",
					i => Directory = i
				},
				{
					$"a|amt|{nameof(AmountToDownload)}=",
					"the amount of images to download.",
					i => SetValue<int>(i, c => AmountToDownload = c)
				},
				{
					$"mw|minw|{nameof(MinWidth)}=",
					"the minimum width to save an image with.",
					i => SetValue<int>(i, c => MinWidth = c)
				},
				{
					$"mh|minh|{nameof(MinHeight)}=",
					"the minimum height to save an image with.",
					i => SetValue<int>(i, c => MinHeight = c)
				},
				{
					$"da|days|{nameof(MaxDaysOld)}=",
					"the oldest an image can be before it won't be saved.",
					i => SetValue<int>(i, c => MaxDaysOld = c)
				},
				{
					$"s|sim|{nameof(MaxImageSimilarity)}=",
					"the percentage similarity before an image should be deleted (1 = .1%, 1000 = 100%).",
					i => SetValue<int>(i, c => MaxImageSimilarity = c)
				},
				{
					$"c|cached|{nameof(ImagesCachedPerThread)}=",
					"how many images to cache on each thread (lower = faster but more CPU).",
					i => SetValue<int>(i, c => ImagesCachedPerThread = c)
				},
				{
					$"csi|compare|{nameof(CompareSavedImages)}=",
					"whether or not to compare to already saved images.",
					i => SetValue<bool>(i, c => CompareSavedImages = c)
				},
			};

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
					Console.WriteLine($"The following parts were extra; was an argument mistyped? '{String.Join("', '", extra)}'");
				}
			}
			catch (FormatException)
			{
				Console.WriteLine("An argument was the invalid type and could not be converted correctly.");
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
		protected void NotifyPropertyChanged(object value, [CallerMemberName] string name = "")
		{
			if (_Arguments.TryGetValue(name, out var property) && !_SetArguments.Any(x => x.Name == name))
			{
				Console.WriteLine($"Successfully set {name} to '{value}'.");
				_SetArguments.Add(property);
			}
			if (!AllArgumentsSet && !_Arguments.Any(x => !_SetArguments.Contains(x.Value)))
			{
				AllArgumentsSet = true;
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		/// <summary>
		/// Attempts to invoke the callback with the string converted to the supplied type, otherwise prints to the console describing what happened.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <param name="callback"></param>
		protected void SetValue<T>(string input, Action<T> callback)
		{
			if (!(TypeDescriptor.GetConverter(typeof(T)) is TypeConverter converter))
			{
				throw new InvalidOperationException($"{typeof(T).Name} is not a valid type to convert to with this method.");
			}

			if (converter.IsValid(input))
			{
				callback((T)converter.ConvertFromInvariantString(input));
			}
			else
			{
				Console.WriteLine($"Unable to convert '{input}' to type {typeof(T).Name}.");
			}
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
		private void DisplayHelp(string input)
		{
			if (CommandLineParserOptions.Contains(input))
			{
				Console.WriteLine($"{input}: {CommandLineParserOptions[input].Description}");
			}
			else
			{
				Console.WriteLine($"'{input}' is not a valid option.");
			}
		}
	}
}
