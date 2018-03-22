using ImageDL.Classes.ImageGatherers;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TPost">The type of each post. Some might be uris, some might be specified classes.</typeparam>
	/// <typeparam name="TImageDetails">The type of image details.</typeparam>
	public abstract class ImageDownloader<TPost> : IImageDownloader
	{
		private const string ANIMATED_CONTENT = "Animated Content";
		private const string FAILED_DOWNLOADS = "Failed Downloads";

		/// <inheritdoc />
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
		/// <inheritdoc />
		public int AmountToDownload
		{
			get => _AmountToDownload;
			set
			{
				_AmountToDownload = Math.Max(1, value);
				NotifyPropertyChanged(_AmountToDownload);
			}
		}
		/// <inheritdoc />
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = Math.Max(0, value);
				NotifyPropertyChanged(_MinWidth);
			}
		}
		/// <inheritdoc />
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = Math.Max(0, value);
				NotifyPropertyChanged(_MinHeight);
			}
		}
		/// <inheritdoc />
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = Math.Max(0, value);
				NotifyPropertyChanged(_MaxDaysOld);
			}
		}
		/// <inheritdoc />
		public int MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = Math.Min(1000, Math.Max(1, value));
				NotifyPropertyChanged(_MaxImageSimilarity);
			}
		}
		/// <inheritdoc />
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set
			{
				_ImagesCachedPerThread = Math.Max(1, value);
				NotifyPropertyChanged(_ImagesCachedPerThread);
			}
		}
		/// <inheritdoc />
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set
			{
				_CompareSavedImages = value;
				NotifyPropertyChanged(_CompareSavedImages);
			}
		}
		/// <inheritdoc />
		public bool Verbose
		{
			get => _Verbose;
			set
			{
				_Verbose = value;
				NotifyPropertyChanged(_Verbose);
			}
		}
		/// <inheritdoc />
		public bool CreateDirectory
		{
			get => _CreateDirectory;
			set
			{
				_CreateDirectory = value;
				NotifyPropertyChanged(_CreateDirectory);
			}
		}
		/// <inheritdoc />
		public bool AllArgumentsSet
		{
			get => _AllArgumentsSet;
			protected set
			{
				_AllArgumentsSet = value;
				NotifyPropertyChanged(_AllArgumentsSet);
			}
		}
		/// <inheritdoc />
		public bool BusyDownloading
		{
			get => _BusyDownloading;
			protected set
			{
				_BusyDownloading = value;
				NotifyPropertyChanged(_BusyDownloading);
			}
		}
		/// <inheritdoc />
		public bool DownloadsFinished
		{
			get => _DownloadsFinished;
			protected set
			{
				_DownloadsFinished = value;
				NotifyPropertyChanged(_DownloadsFinished);
			}
		}
		/// <inheritdoc />
		public List<WebsiteScraper> Scrapers { get; protected set; } = typeof(IImageDownloader).Assembly.DefinedTypes
			.Where(x => x.IsSubclassOf(typeof(WebsiteScraper)))
			.Select(x => Activator.CreateInstance(x))
			.Cast<WebsiteScraper>()
			.ToList();
		/// <inheritdoc />
		public DateTime OldestAllowed => DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
		/// <inheritdoc />
		public IImageComparer ImageComparer
		{
			get => _ImageComparer;
			set => _ImageComparer = value;
		}

		protected ImmutableArray<PropertyInfo> Arguments;
		protected List<PropertyInfo> ModifiedArguments = new List<PropertyInfo>();
		protected List<ContentLink> Links = new List<ContentLink>();
		protected OptionSet CommandLineParserOptions;

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
		private IImageComparer _ImageComparer;

		/// <summary>
		/// Indicates when a setting has been set.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public ImageDownloader()
		{
			CommandLineParserOptions = GetCommandLineParserOptions();
			Arguments = GetArguments();

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

		/// <inheritdoc />
		public async Task StartAsync()
		{
			AllArgumentsSet = false;
			BusyDownloading = true;

			var posts = await GatherPostsAsync().ConfigureAwait(false);
			if (!posts.Any())
			{
				Console.WriteLine("Unable to find any posts matching the search criteria.");
				return;
			}

			var count = 0;
			foreach (var post in posts)
			{
				WritePostToConsole(post, ++count);

				var gatherer = await CreateGathererAsync(post).ConfigureAwait(false);
				foreach (var imageUri in gatherer.ImageUris)
				{
					await Task.Delay(100).ConfigureAwait(false);
					try
					{
						Console.WriteLine($"\t{await DownloadImageAsync(gatherer, post, imageUri).ConfigureAwait(false)}");
					}
					//Catch all so they can be written and logged as a failed download
					catch (Exception e)
					{
						e.Write();
						Links.Add(CreateContentLink(post, imageUri, FAILED_DOWNLOADS));
					}
				}
			}
			BusyDownloading = false;

			//No point in trying to cache images or delete duplicates if
			//a) image comparer doesn't exist to do that
			//b) nothing new was saved
			if (ImageComparer != null && ImageComparer.StoredImages > 0)
			{
				if (CompareSavedImages)
				{
					Console.WriteLine();
					await ImageComparer.CacheSavedFilesAsync(new DirectoryInfo(Directory), ImagesCachedPerThread);
				}
				Console.WriteLine();
				ImageComparer.DeleteDuplicates(MaxImageSimilarity / 1000f);
				Console.WriteLine();
			}
			DownloadsFinished = true;

			SaveStoredContentLinks();
		}
		/// <inheritdoc />
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
		/// <inheritdoc />
		public void AskForArguments()
		{
			var unsetArgs = Arguments.Where(x => !ModifiedArguments.Contains(x));
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
		/// Downloads an image from <paramref name="uri"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		protected async Task<string> DownloadImageAsync(ImageGatherer gatherer, TPost post, Uri uri)
		{
			if (!String.IsNullOrWhiteSpace(gatherer.Error))
			{
				return gatherer.Error;
			}
			else if (gatherer.IsAnimated)
			{
				Links.Add(CreateContentLink(post, gatherer.OriginalUri, ANIMATED_CONTENT));
				return $"{gatherer.OriginalUri} is animated content (gif/video).";
			}

			WebResponse resp = null;
			Stream rs = null;
			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				resp = await uri.CreateWebRequest().GetResponseAsync().ConfigureAwait(false);
				if (resp.ContentType.Contains("video/") || resp.ContentType == "image/gif")
				{
					Links.Add(CreateContentLink(post, uri, ANIMATED_CONTENT));
					return $"{uri} is animated content (gif/video).";
				}
				if (!resp.ContentType.Contains("image/"))
				{
					return $"{uri} is not an image.";
				}
				var file = GenerateFileInfo(post, resp, uri);
				if (File.Exists(file.FullName))
				{
					return $"{uri} is already saved as {file}.";
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating a bitmap ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = resp.GetResponseStream()).CopyToAsync(ms = new MemoryStream()).ConfigureAwait(false);

				//If image is too small, don't bother saving
				var (width, height) = ms.GetImageSize();
				if (width < MinWidth || height < MinHeight)
				{
					return $"{uri} is too small ({width}x{height}).";
				}
				//If the image comparer returns any errors when trying to store, then return that error
				if (ImageComparer != null && !ImageComparer.TryStore(uri, file, ms, width, height, out var error))
				{
					return error;
				}

				//Save the file
				ms.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs = file.Create()).ConfigureAwait(false);
				return $"Saved {uri} to {file}.";
			}
			finally
			{
				resp?.Dispose();
				rs?.Dispose();
				ms?.Dispose();
				fs?.Dispose();
			}
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
		/// Saves the stored content links to file.
		/// </summary>
		protected void SaveStoredContentLinks()
		{
			foreach (var kvp in Links.GroupBy(x => x.Reason))
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
		/// <summary>
		/// Invokes <see cref="PropertyChanged"/>.
		/// </summary>
		/// <param name="name">The property changed.</param>
		protected void NotifyPropertyChanged(object value, [CallerMemberName] string name = "")
		{
			if (!ModifiedArguments.Any(x => x.Name == name) && Arguments.SingleOrDefault(x => x.Name == name) is PropertyInfo prop)
			{
				ModifiedArguments.Add(prop);
				if (_Verbose)
				{
					Console.WriteLine($"Successfully set {name} to '{value}'.");
				}
			}
			if (!AllArgumentsSet && !Arguments.Any(x => !ModifiedArguments.Contains(x)))
			{
				AllArgumentsSet = true;
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		/// <summary>
		/// Returns the default options that are used to set values through console input.
		/// </summary>
		/// <returns></returns>
		private OptionSet GetCommandLineParserOptions()
		{
			return new OptionSet
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
		}
		/// <summary>
		/// Gets the names of settings (public, instance, has setter, has getter, and is a property).
		/// </summary>
		/// <returns></returns>
		private ImmutableArray<PropertyInfo> GetArguments()
		{
			return GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.GetSetMethod() != null && x.GetGetMethod() != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToImmutableArray();
		}
		/// <summary>
		/// Displays help for whatever option has the supplied key.
		/// </summary>
		/// <param name="input"></param>
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

		/// <summary>
		/// Gathers the posts which match the supplied settings.
		/// </summary>
		/// <returns></returns>
		protected abstract Task<List<TPost>> GatherPostsAsync();
		/// <summary>
		/// Writes the post to the console indicating it is being downloaded.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="count"></param>
		protected abstract void WritePostToConsole(TPost post, int count);
		/// <summary>
		/// Generate a filename to save an image with.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="response"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		protected abstract FileInfo GenerateFileInfo(TPost post, WebResponse response, Uri uri);
		/// <summary>
		/// Scrape images from a post.
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		protected abstract Task<ImageGatherer> CreateGathererAsync(TPost post);
		/// <summary>
		/// Store information about an image from a post.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="uri"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri, string reason);
	}
}
