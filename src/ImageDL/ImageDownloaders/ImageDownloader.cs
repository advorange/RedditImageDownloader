using ImageDL.Classes;
using ImageDL.Utilities;
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

		[Setting("The location to save files to.")]
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
		[Setting("The amount of files to download.")]
		public int AmountToDownload
		{
			get => Math.Max(1, _AmountToDownload);
			set
			{
				_AmountToDownload = value;
				NotifyPropertyChanged();
			}
		}
		[Setting("The minimum width for an image to have allowing it to be saved.")]
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = value;
				NotifyPropertyChanged();
			}
		}
		[Setting("The minimum height for an image to have allowing it to be saved.")]
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = value;
				NotifyPropertyChanged();
			}
		}
		[Setting("The maximum age a post can have before it is not longer allowed to be saved.")]
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = value;
				NotifyPropertyChanged();
			}
		}
		[Setting("The maximum acceptable percentage for image similarity before images are detected as duplicates. " +
			"Ranges from 1 to 1000 (acts as .1% similar to 100% similar).")]
		public int MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = Math.Min(1000, Math.Max(1, value));
				NotifyPropertyChanged();
			}
		}
		[Setting("The amount of images to cache per thread. Lower value means faster at the cost of higher CPU usage.")]
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set
			{
				_ImagesCachedPerThread = value;
				NotifyPropertyChanged();
			}
		}
		[Setting("Whether or not to include already saved images in comparison for duplicates.")]
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

		public ImageDownloader() : this(new string[0]) { }
		public ImageDownloader(params string[] args)
		{
			_Arguments = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.GetCustomAttribute<SettingAttribute>() != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToImmutableDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

			MaxImageSimilarity = 1000;
			ImagesCachedPerThread = 50;
			CompareSavedImages = false;

			//Save on close in case program is closed while running
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => SaveStoredContentLinks();

			SetArguments(args);
		}

		/// <summary>
		/// Start downloading images.
		/// </summary>
		/// <returns>An asynchronous task which downloads images.</returns>
		public abstract Task StartAsync();
		/// <summary>
		/// Gives help for each argument that help has been asked for.
		/// </summary>
		/// <param name="argNames">The argument names to search for.</param>
		public void GiveHelp(params string[] argNames)
		{
			foreach (var argName in argNames)
			{
				var text = _Arguments.TryGetValue(argName, out var property)
					? $"{property.Name}: {property.GetCustomAttribute<SettingAttribute>().Description}"
					: $"{argName} is not a valid argument name.";
				Console.WriteLine(text);
			}
		}
		/// <summary>
		/// Sets the arguments that can be gathered from <paramref name="args"/>.
		/// </summary>
		/// <param name="args">The supplied arguments.</param>
		public void SetArguments(params string[] args)
		{
			foreach (var argument in args)
			{
				Console.WriteLine(SetArgument(argument));
			}
			if (args.Any())
			{
				Console.WriteLine();
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
			if (!_SetArguments.Any(x => x.Name == name) && _Arguments.TryGetValue(name, out var property))
			{
				_SetArguments.Add(property);
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private string SetArgument(string argument)
		{
			//Split, left side is the arg name, right is value
			var split = argument.Split(new[] { ':' }, 2);
			if (split.Length != 2)
			{
				return $"Unable to split \"{argument}\" to the correct length.";
			}

			//See if any arguments have the supplied name
			if (!_Arguments.TryGetValue(split[0], out var property))
			{
				return $"{split[0]} is not a valid argument name.";
			}
			else if (String.IsNullOrWhiteSpace(split[1]))
			{
				return $"Failed to set {property.Name}. Reason: may not have an empty value.";
			}
			else if (TypeDescriptor.GetConverter(property.PropertyType) is TypeConverter converter)
			{
				try
				{
					property.SetValue(this, converter.ConvertFromInvariantString(split[1]));
				}
				catch
				{
					return $"Failed to set {property.Name}. Reason: invalid value supplied.";
				}
			}
			else if (property.PropertyType == typeof(string))
			{
				property.SetValue(this, split[1]);
			}
			else
			{
				return $"Failed to set {property.Name}. Reason: invalid type (not user error).";
			}

			if (!_Arguments.Where(x => !_SetArguments.Contains(x.Value)).Any())
			{
				AllArgumentsSet = true;
			}
			return $"Successfully set {property.Name} to {property.GetValue(this)}.";
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
