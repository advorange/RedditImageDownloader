using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using ImageDL.Classes.SettingParsing.Converting;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TPost">The type of each post. Some might be uris, some might be specified classes.</typeparam>
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
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int AmountToDownload
		{
			get => _AmountToDownload;
			set
			{
				_AmountToDownload = Math.Max(1, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = Math.Max(0, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = Math.Max(0, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = Math.Max(0, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public Percentage MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set
			{
				_ImagesCachedPerThread = Math.Max(1, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public int MinScore
		{
			get => _MinScore;
			set
			{
				_MinScore = Math.Max(0, value);
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public AspectRatio MinAspectRatio
		{
			get => _MinAspectRatio;
			set
			{
				_MinAspectRatio = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public AspectRatio MaxAspectRatio
		{
			get => _MaxAspectRatio;
			set
			{
				_MaxAspectRatio = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set
			{
				_CompareSavedImages = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public bool Verbose
		{
			get => _Verbose;
			set
			{
				_Verbose = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public bool CreateDirectory
		{
			get => _CreateDirectory;
			set
			{
				_CreateDirectory = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public bool Start
		{
			get => _Start;
			set
			{
				_Start = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc />
		public DateTime OldestAllowed => DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
		/// <inheritdoc />
		public IImageComparer ImageComparer
		{
			get => _ImageComparer;
			set
			{
				_ImageComparer = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		/// Links to content that is animated, failed to download, etc.
		/// </summary>
		protected List<ContentLink> Links = new List<ContentLink>();
		/// <summary>
		/// Used to set arguments via command line.
		/// </summary>
		protected SettingParser SettingParser;
		/// <summary>
		/// Used to download files.
		/// </summary>
		protected ImageDownloaderClient Client;
		/// <summary>
		/// To make sure only one instance is running at a time.
		/// </summary>
		protected SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

		private string _Directory;
		private int _AmountToDownload;
		private int _MinWidth;
		private int _MinHeight;
		private int _MaxDaysOld;
		private Percentage _MaxImageSimilarity;
		private int _ImagesCachedPerThread;
		private int _MinScore;
		private AspectRatio _MinAspectRatio;
		private AspectRatio _MaxAspectRatio;
		private bool _CompareSavedImages;
		private bool _Verbose;
		private bool _CreateDirectory;
		private bool _Start;
		private IImageComparer _ImageComparer;

		/// <summary>
		/// Indicates when a setting has been set.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Creates an image downloader.
		/// </summary>
		public ImageDownloader()
		{
			SettingParser = GetSettingParser();
			Client = new ImageDownloaderClient();

			//Save on close in case program is closed while running
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => SaveStoredContentLinks();
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
		}

		/// <summary>
		/// Returns a file info with a maximum path length of 255 characters.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="name"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static FileInfo GenerateFileInfo(string directory, string name, string extension)
		{
			//Make sure the extension has a period
			extension = extension.StartsWith(".") ? extension : "." + extension;
			//Remove any invalid file name path characters
			name = new string(name.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
			//Max file name length has to be under 260 for windows, but 256 for some other things, so just go with 255.
			var nameLen = 255 - directory.Length - 1 - extension.Length; //Subtract extra 1 for / between dir and file
			//Cut the file name down to its valid length so no length errors occur
			return new FileInfo(Path.Combine(directory, name.Substring(0, Math.Min(name.Length, nameLen)) + extension));
		}

		/// <inheritdoc />
		public bool CanStart()
		{
			return Start && SettingParser.AllSet;
		}
		/// <inheritdoc />
		public async Task StartAsync(CancellationToken token = default)
		{
			await SemaphoreSlim.WaitAsync(token).CAF();

			Console.WriteLine();
			var posts = await GatherPostsAsync().CAF();
			if (!posts.Any())
			{
				Console.WriteLine("Unable to find any posts matching the search criteria.");
				return;
			}

			var count = 0;
			foreach (var post in posts)
			{
				token.ThrowIfCancellationRequested();
				WritePostToConsole(post, ++count);

				var gatherer = await GatherImagesAsync(post).CAF();
				foreach (var imageUri in gatherer.ImageUris)
				{
					await Task.Delay(100).CAF();
					try
					{
						Console.WriteLine($"\t{await DownloadImageAsync(gatherer, post, imageUri).CAF()}");
					}
					//Catch all so they can be written and logged as a failed download
					catch (Exception e)
					{
						e.Write();
						Links.Add(CreateContentLink(post, imageUri, FAILED_DOWNLOADS));
					}
				}
			}

			//No point in trying to cache images or delete duplicates if
			//a) image comparer doesn't exist to do that
			//b) nothing new was saved
			if (ImageComparer != null && ImageComparer.StoredImages > 0)
			{
				if (CompareSavedImages)
				{
					Console.WriteLine();
					await ImageComparer.CacheSavedFilesAsync(new DirectoryInfo(Directory), ImagesCachedPerThread, token);
				}
				Console.WriteLine();
				ImageComparer.DeleteDuplicates(MaxImageSimilarity);
				Console.WriteLine();
			}

			SaveStoredContentLinks();
			SemaphoreSlim.Release();
		}
		/// <inheritdoc />
		public void SetArguments(string input)
		{
			var result = SettingParser.Parse(input.SplitLikeCommandLine());
			if (!(result.Successes.Any() || result.Errors.Any() || result.UnusedParts.Any() || result.Help.Any()))
			{
				return;
			}

			var wentIntoAny = false;
			if (result.Help.Any(x => x != null))
			{
				Console.WriteLine(String.Join("\n", result.Help));
				wentIntoAny = true;
			}
			if (Verbose && result.Successes.Any(x => x != null))
			{
				Console.WriteLine(String.Join("\n", result.Successes));
				wentIntoAny = true;
			}
			if (Verbose && result.Errors.Any(x => x != null))
			{
				Console.WriteLine($"The following errors occurred:\n{String.Join("\n\t", result.Errors)}");
				wentIntoAny = true;
			}
			if (Verbose && result.UnusedParts.Any(x => x != null))
			{
				Console.WriteLine($"The following parts were extra; was an argument mistyped? '{String.Join("', '", result.UnusedParts)}'");
				wentIntoAny = true;
			}
			if (wentIntoAny)
			{
				Console.WriteLine();
			}
		}
		/// <inheritdoc />
		public void AskForArguments()
		{
			var unsetArguments = SettingParser.GetNeededSettings();
			if (!unsetArguments.Any())
			{
				Console.WriteLine($"Every setting which is necessary has been set.");
				return;
			}

			var sb = new StringBuilder("The following settings need to be set:" + Environment.NewLine);
			foreach (var setting in unsetArguments)
			{
				sb.AppendLine($"\t{setting.ToString()}");
			}
			Console.WriteLine(sb.ToString().Trim());
			Console.WriteLine();
		}
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="result">The gathered image uris.</param>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		protected async Task<string> DownloadImageAsync(ScrapeResult result, TPost post, Uri uri)
		{
			if (!String.IsNullOrWhiteSpace(result.Error))
			{
				if (result.IsAnimated)
				{
					Links.Add(CreateContentLink(post, result.OriginalUri, ANIMATED_CONTENT));
				}
				return result.Error;
			}

			HttpResponseMessage resp = null;
			Stream rs = null;
			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				resp = await Client.SendWithRefererAsync(uri, HttpMethod.Get).CAF();
				var contentType = resp.Content.Headers.GetValues("Content-Type").First();
				if (contentType.Contains("video/") || contentType == "image/gif")
				{
					Links.Add(CreateContentLink(post, uri, ANIMATED_CONTENT));
					return $"{uri} is animated content (gif/video).";
				}
				if (!contentType.Contains("image/"))
				{
					return $"{uri} is not an image.";
				}
				var file = GenerateFileInfo(post, resp.Headers.Location ?? uri);
				if (File.Exists(file.FullName))
				{
					return $"{uri} is already saved as {file}.";
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating an image ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = await resp.Content.ReadAsStreamAsync().CAF()).CopyToAsync(ms = new MemoryStream());

				//If image is too small, don't bother saving
				var (width, height) = ms.GetImageSize();
				if (!FitsSizeRequirements(uri, width, height, out var sizeError))
				{
					return sizeError;
				}
				//If the image comparer returns any errors when trying to store, then return that error
				if (ImageComparer != null && !ImageComparer.TryStore(uri, file, ms, width, height, out var cachingError))
				{
					return cachingError;
				}

				//Save the file
				ms.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs = file.Create()).CAF();
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
		/// Checks min width, min height, and the min/max aspect ratios.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected bool FitsSizeRequirements(Uri uri, int width, int height, out string error)
		{
			//Check min width/height
			if (width < MinWidth || height < MinHeight)
			{
				error = $"{uri} is too small ({width}x{height}).";
				return false;
			}
			//Check aspect ratio
			var aspectRatio = width / (float)height;
			if (aspectRatio < MinAspectRatio.Value || aspectRatio > MaxAspectRatio.Value)
			{
				error = $"{uri} does not fit in the aspect ratio restrictions ({width}x{height}).";
				return false;
			}
			error = null;
			return true;
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
				var format = unsavedContent.OrderByDescending(x => x.AssociatedNumber)
					.Select(x => $"{x.AssociatedNumber.ToString().PadLeft(len, '0')} {x.Uri}");
				using (var writer = file.AppendText())
				{
					writer.WriteLine($"{kvp.Key.FormatTitle()} - {Formatting.ToSaving()}");
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
		protected void NotifyPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		/// <summary>
		/// Returns the default options that are used to set values through console input.
		/// </summary>
		/// <returns></returns>
		private SettingParser GetSettingParser()
		{
			return new SettingParser(new[] { "--", "-", "/" })
			{
				new Setting<string>(new[] { nameof(Directory), "dir" }, x => Directory = x)
				{
					Description = "The directory to save to.",
				},
				new Setting<int>(new[] {nameof(AmountToDownload), "amt"}, x => AmountToDownload = x)
				{
					Description = "The amount of images to download.",
				},
				new Setting<int>(new[] {nameof(MinWidth), "minw", "mw" }, x => MinWidth = x)
				{
					Description = "The minimum width to save an image with.",
				},
				new Setting<int>(new[] {nameof(MinHeight), "minh", "mh" }, x => MinHeight = x)
				{
					Description = "The minimum height to save an image with.",
				},
				new Setting<int>(new[] {nameof(MaxDaysOld), "age" }, x => MaxDaysOld = x)
				{
					Description = "The oldest an image can be before it won't be saved.",
				},
				new Setting<Percentage>(new[] {nameof(MaxImageSimilarity), "sim" }, x => MaxImageSimilarity = x, new PercentageConverter())
				{
					Description = "The percentage similarity before an image should be deleted (1 = .1%, 1000 = 100%).",
					DefaultValue = new Percentage(1),
				},
				new Setting<int>(new[] {nameof(ImagesCachedPerThread), "icpt" }, x => ImagesCachedPerThread = x)
				{
					Description = "How many images to cache on each thread (lower = faster but more CPU).",
					DefaultValue = 50,
				},
				new Setting<int>(new[] {nameof(MinScore), "mins", "ms" }, x => MinScore = x)
				{
					Description = "The minimum score for an image to have before being ignored.",
					IsOptional = true,
				},
				new Setting<AspectRatio>(new[] {nameof(MinAspectRatio), "minar" }, x => MinAspectRatio = x, new AspectRatioConverter())
				{
					Description = "The minimum aspect ratio for an image to have before being ignored.",
					DefaultValue = new AspectRatio(0, 1),
				},
				new Setting<AspectRatio>(new[] {nameof(MaxAspectRatio), "maxar" }, x => MaxAspectRatio = x, new AspectRatioConverter())
				{
					Description = "The maximum aspect ratio for an image to have before being ignored.",
					DefaultValue = new AspectRatio(1, 0),
				},
				new Setting<bool>(new[] {nameof(CompareSavedImages), "csi" }, x => CompareSavedImages = x)
				{
					Description = "Whether or not to compare to already saved images.",
					IsFlag = true,
					IsOptional = true,
				},
				new Setting<bool>(new[] {nameof(CreateDirectory), "create", "cd" }, x => CreateDirectory = x)
				{
					Description = "Whether or not to create the directory if it does not exist.",
					IsFlag = true,
					IsOptional = true,
				},
				new Setting<bool>(new[] {nameof(Verbose), "v" }, x => Verbose = x)
				{
					Description = "Whether or not to print extra information to the console, such as variables being set.",
					IsFlag = true,
					IsOptional = true,
				},
				new Setting<bool>(new[] {nameof(Start), "s" }, x => Start = x)
				{
					Description = "Whether or not to start the downloader. Will not start until all other arguments are provided.",
					IsFlag = true,
				},
			};
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
		/// <param name="uri"></param>
		/// <returns></returns>
		protected abstract FileInfo GenerateFileInfo(TPost post, Uri uri);
		/// <summary>
		/// Scrape images from a post.
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		protected abstract Task<ScrapeResult> GatherImagesAsync(TPost post);
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