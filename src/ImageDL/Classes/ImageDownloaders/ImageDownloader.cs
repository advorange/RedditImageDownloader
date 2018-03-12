using ImageDL.Classes.ImageGatherers;
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

namespace ImageDL.Classes.ImageDownloaders
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
				if (!_CreateDirectory && !System.IO.Directory.Exists(value))
				{
					throw new ArgumentException($"{value} is not already created and -cd has not been used.", nameof(Directory));
				}
				else if (System.IO.Directory.CreateDirectory(value) is null)
				{
					throw new ArgumentException($"{value} is an invalid directory name.", nameof(Directory));
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
		/// Indicates whether or not to print extra information to the console. Such as variables being set.
		/// </summary>
		public bool Verbose
		{
			get => _Verbose;
			set
			{
				_Verbose = value;
				NotifyPropertyChanged(_Verbose);
			}
		}
		/// <summary>
		/// Indicates whether or not to create the directory if it does not exist.
		/// </summary>
		public bool CreateDirectory
		{
			get => _CreateDirectory;
			set
			{
				_CreateDirectory = value;
				NotifyPropertyChanged(_CreateDirectory);
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
		/// How to scrape from specific websites.
		/// </summary>
		public List<WebsiteScraper> Scrapers = new List<WebsiteScraper>
		{
			new DeviantArtScraper(),
			new ImgurScraper(),
			new InstagramScraper(),
			new PixivScraper(),
			new TumblrScraper(),
		};
		/// <summary>
		/// Indicates when a setting has been set.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		protected ImmutableArray<PropertyInfo> _Arguments;
		protected List<PropertyInfo> _SetArguments = new List<PropertyInfo>();
		protected List<ContentLink> _Links = new List<ContentLink>();
		protected ImageComparer _ImageComparer;

		private string _Directory;
		private int _AmountToDownload;
		private int _MinWidth;
		private int _MinHeight;
		private int _MaxDaysOld;
		private int _MaxImageSimilarity;
		private int _ImagesCachedPerThread;
		private bool _CompareSavedImages;
		private bool _Verbose;
		private bool _CreateDirectory;
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
					i => DisplayHelp(i)
				},
				{
					$"dir|{nameof(Directory)}=",
					"the directory to save to.",
					i => SetValue<string>(i, c => Directory = c)
				},
				{
					$"amt|{nameof(AmountToDownload)}=",
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
					$"age|{nameof(MaxDaysOld)}=",
					"the oldest an image can be before it won't be saved.",
					i => SetValue<int>(i, c => MaxDaysOld = c)
				},
				{
					$"sim|{nameof(MaxImageSimilarity)}=",
					"the percentage similarity before an image should be deleted (1 = .1%, 1000 = 100%).",
					i => SetValue<int>(i, c => MaxImageSimilarity = c)
				},
				{
					$"icpt|{nameof(ImagesCachedPerThread)}=",
					"how many images to cache on each thread (lower = faster but more CPU).",
					i => SetValue<int>(i, c => ImagesCachedPerThread = c)
				},
				{
					$"csi|{nameof(CompareSavedImages)}=",
					"whether or not to compare to already saved images.",
					i => SetValue<bool>(i, c => CompareSavedImages = c)
				},
				{
					$"cd|create|{nameof(CreateDirectory)}:",
					"whether or not to create the directory if it does not exist.",
					i => SetValue<bool>(i, c => CreateDirectory = c, true, true)
				},
				{
					$"v|{nameof(Verbose)}:",
					"whether or not to print extra information to the console, such as variables being set.",
					i => SetValue<bool>(i, c => Verbose = c, true, true)
				}
			};

			_Arguments = GetArguments();

			//Set verbose to false so these settings don't print
			//These settings are default values, but need to be set from here so NotifyPropertyChanged adds them to the set values
			Verbose = false;
			CreateDirectory = false;
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
			var unsetArgs = _Arguments.Where(x => !_SetArguments.Contains(x));
			if (!unsetArgs.Any())
			{
				return;
			}

			var sb = new StringBuilder("The following arguments need to be set:" + Environment.NewLine);
			foreach (var prop in unsetArgs)
			{
				sb.AppendLine($"\t{prop.Name} ({prop.PropertyType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}

		/// <summary>
		/// Invokes <see cref="PropertyChanged"/>.
		/// </summary>
		/// <param name="name">The property changed.</param>
		protected void NotifyPropertyChanged(object value, [CallerMemberName] string name = "")
		{
			if (!_SetArguments.Any(x => x.Name == name) && _Arguments.SingleOrDefault(x => x.Name == name) is PropertyInfo prop)
			{
				_SetArguments.Add(prop);
				if (_Verbose)
				{
					Console.WriteLine($"Successfully set {name} to '{value}'.");
				}
			}
			if (!AllArgumentsSet && !_Arguments.Any(x => !_SetArguments.Contains(x)))
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
		protected void SetValue<T>(string input, Action<T> callback, bool useDefaultIfNull = false, T defaultValue = default)
		{
			T val;
			if (input == null && useDefaultIfNull)
			{
				val = defaultValue;
			}
			else if (TypeDescriptor.GetConverter(typeof(T)) is TypeConverter converter && converter.IsValid(input))
			{
				val = (T)converter.ConvertFromInvariantString(input);
			}
			else
			{
				Console.WriteLine($"Unable to convert '{input}' to type {typeof(T).Name}.");
				return;
			}

			try
			{
				callback(val);
			}
			catch (ArgumentException e)
			{
				e.Write();
			}
		}
		/// <summary>
		/// Gets the names of settings (public, instance, has setter, has getter, and is a property).
		/// </summary>
		/// <returns></returns>
		protected ImmutableArray<PropertyInfo> GetArguments()
		{
			return GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.GetSetMethod() != null && x.GetGetMethod() != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToImmutableArray();
		}
		/// <summary>
		/// Saves the stored content links to file.
		/// </summary>
		protected virtual void SaveStoredContentLinks()
		{
			foreach (var kvp in _Links.GroupBy(x => x.Reason))
			{
				//Only save links which are not already in the text document
				var file = new FileInfo(Path.Combine(Directory, $"{kvp.Key}.txt"));
				var unsavedContent = new List<ContentLink>();
				if (file.Exists)
				{
					using (var reader = new StreamReader(file.OpenRead()))
					{
						var text = reader.ReadToEnd();
						foreach (var anim in kvp)
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
					unsavedContent = kvp.ToList();
				}
				if (!unsavedContent.Any())
				{
					continue;
				}

				//Save all the links then say how many were saved
				var len = unsavedContent.Max(x => x.AssociatedNumber).ToString().Length;
				var format = unsavedContent.OrderByDescending(x => x.AssociatedNumber).Select(x => $"{x.AssociatedNumber.ToString().PadLeft(len, '0')} {x.Uri}");
				using (var writer = file.AppendText())
				{
					writer.WriteLine($"{kvp.Key.FormatTitle()} - {Utils.FormatDateTimeForSaving()}");
					writer.WriteLine(String.Join(Environment.NewLine, format));
					writer.WriteLine();
				}
				Console.WriteLine($"Added {unsavedContent.Count()} links to {file}.");
			}
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
