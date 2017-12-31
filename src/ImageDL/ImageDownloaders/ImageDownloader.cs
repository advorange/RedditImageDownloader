using ImageDL.Classes;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TPost">The type of each post. Some might be uris, some might be specified classes.</typeparam>
	public abstract class ImageDownloader<TPost> : IImageDownloader
	{
		protected static Dictionary<Type, Func<string, object>> _TryParses = new Dictionary<Type, Func<string, object>>
		{
			{ typeof(bool), (value) => bool.TryParse(value, out var result) ? result : false },
			{ typeof(int), (value) => int.TryParse(value, out var result) ? result : default },
			{ typeof(int?), (value) => int.TryParse(value, out var result) ? result as int? : null },
			{ typeof(uint), (value) => uint.TryParse(value, out var result) ? result : default },
			{ typeof(uint?), (value) => uint.TryParse(value, out var result) ? result as uint? : null },
			{ typeof(long), (value) => long.TryParse(value, out var result) ? result : default },
			{ typeof(long?), (value) => long.TryParse(value, out var result) ? result as long? : null },
			{ typeof(ulong), (value) => ulong.TryParse(value, out var result) ? result : default },
			{ typeof(ulong?), (value) => ulong.TryParse(value, out var result) ? result as ulong? : null },
			{ typeof(float), (value) => float.TryParse(value, out var result) ? result : default },
			{ typeof(float?), (value) => float.TryParse(value, out var result) ? result as float? : null },
		};

		public event Func<Task> AllArgumentsSet;
		public event Func<Task> DownloadsFinished;

		private string _Directory;
		[Setting("The location to save files to.")]
		public string Directory
		{
			get => _Directory;
			set
			{
				_Directory = value;
				AddArgumentToSetArguments();
			}
		}
		private int _AmountToDownload;
		[Setting("The amount of files to download.")]
		public int AmountToDownload
		{
			get => Math.Max(1, _AmountToDownload);
			set
			{
				_AmountToDownload = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MinWidth;
		[Setting("The minimum width for an image to have allowing it to be saved.")]
		public int MinWidth
		{
			get => _MinWidth;
			set
			{
				_MinWidth = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MinHeight;
		[Setting("The minimum height for an image to have allowing it to be saved.")]
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MaxDaysOld;
		[Setting("The maximum age a post can have before it is not longer allowed to be saved.")]
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set
			{
				_MaxDaysOld = value;
				AddArgumentToSetArguments();
			}
		}
		private int _MaxImageSimilarity;
		[Setting("The maximum acceptable percentage for image similarity before images are detected as duplicates. Ranges from 1 to 100.")]
		public int MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set
			{
				_MaxImageSimilarity = Math.Min(100, Math.Max(1, value));
				AddArgumentToSetArguments();
			}
		}
		private bool _CompareSavedImages;
		[Setting("Whether or not to include already saved images in comparison for duplicates.")]
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set
			{
				_CompareSavedImages = value;
				AddArgumentToSetArguments();
			}
		}

		protected ImmutableList<PropertyInfo> _Arguments;
		protected List<PropertyInfo> _SetArguments = new List<PropertyInfo>();
		protected List<ContentLink> _AnimatedContent = new List<ContentLink>();
		protected List<ContentLink> _FailedDownloads = new List<ContentLink>();
		protected ImageComparer _ImageComparer;

		public ImageDownloader(params string[] args)
		{
			_Arguments = GetSettings();
			if (args.Any())
			{
				AddArguments(args);
			}

			CompareSavedImages = false;
		}

		/// <summary>
		/// Gives help for each argument that help has been asked for.
		/// </summary>
		/// <param name="argNames">The argument names to search for.</param>
		public void GiveHelp(params string[] argNames)
		{
			foreach (var argName in argNames)
			{
				var arg = _Arguments.SingleOrDefault(x => x.Name.CaseInsEquals(argName));
				var text = arg == null
					? $"{argName} is not a valid argument name."
					: $"{arg.Name}: {arg.GetCustomAttribute<SettingAttribute>().Description}";
				Console.WriteLine(text);
			}
		}
		/// <summary>
		/// Sets the arguments that can be gathered from <paramref name="args"/>.
		/// </summary>
		/// <param name="args">The supplied arguments.</param>
		public void AddArguments(params string[] args)
		{
			foreach (var argument in args)
			{
				//Split, left side is the arg name, right is value
				var split = argument.Split(new[] { ':' }, 2);
				if (split.Length != 2)
				{
					Console.WriteLine($"Unable to split \"{argument}\" to the correct length.");
					continue;
				}

				//See if any arguments have the supplied name
				var property = _Arguments.SingleOrDefault(x => x.Name.CaseInsEquals(split[0]));
				if (property == null)
				{
					Console.WriteLine($"{split[0]} is not a valid argument name.");
					continue;
				}
				else if (String.IsNullOrWhiteSpace(split[1]))
				{
					Console.WriteLine($"{property.Name} may not have an empty value.");
					continue;
				}
				else if (_TryParses.TryGetValue(property.PropertyType, out var f))
				{
					property.SetValue(this, f(split[1]));
				}
				else if (property.PropertyType == typeof(string))
				{
					property.SetValue(this, split[1]);
				}
				else
				{
					Console.WriteLine($"Unable to set the value for {property.Name}.");
					continue;
				}

				Console.WriteLine($"Successfully set {property.Name} to {property.GetValue(this)}.");
			}
			Console.WriteLine();

			if (!String.IsNullOrWhiteSpace(Directory) && !System.IO.Directory.Exists(Directory))
			{
				Console.WriteLine($"{Directory} does not exist as a directory.");
			}
			else if (!_Arguments.Where(x => !_SetArguments.Contains(x)).Any())
			{
				AllArgumentsSet?.Invoke();
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
			foreach (var argument in unsetArgs)
			{
				sb.AppendLine($"\t{argument.Name} ({argument.PropertyType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}
		/// <summary>
		/// Adds a <see cref="PropertyInfo"/> with the supplied name to <see cref="_SetArguments"/>.
		/// </summary>
		/// <param name="name">The property to find.</param>
		protected void AddArgumentToSetArguments([CallerMemberName] string name = "")
		{
			if (!_SetArguments.Any(x => x.Name == name))
			{
				_SetArguments.Add(_Arguments.Single(x => x.Name == name));
			}
		}
		/// <summary>
		/// Returns an array containing settings from the supplied type.
		/// A setting is a public instance property that is either primitive or string with a set method.
		/// </summary>
		/// <param name="type">The type to gather settings from.</param>
		/// <returns>An array containing settings.</returns>
		protected ImmutableList<PropertyInfo> GetSettings()
			=> GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.GetCustomAttribute<SettingAttribute>() != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToImmutableList();

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		public async Task StartAsync()
		{
			_ImageComparer = new ImageComparer
			{
				ThumbnailSize = 64,
			};
			if (CompareSavedImages)
			{
				_ImageComparer.CacheAlreadySavedFiles(new DirectoryInfo(Directory));
			}

			var count = 0;
			foreach (var post in await GatherPostsAsync())
			{
				WritePostToConsole(post, ++count);
				var gatherer = await CreateGatherer(post).ConfigureAwait(false);
				foreach (var imageUri in gatherer.ImageUris)
				{
					await Task.Delay(100).ConfigureAwait(false);
					await DownloadImageAsync(post, imageUri).ConfigureAwait(false);
				}
			}

			if (_AnimatedContent.Any())
			{
				SaveContentLinks(_AnimatedContent, new FileInfo(Path.Combine(Directory, "Animated_Content.txt")));
			}
			if (_FailedDownloads.Any())
			{
				SaveContentLinks(_FailedDownloads, new FileInfo(Path.Combine(Directory, "Failed_Downloads.txt")));
			}
			_ImageComparer.DeleteDuplicates(MaxImageSimilarity / 100f);
			DownloadsFinished?.Invoke();
		}
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it.
		/// </summary>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns></returns>
		public async Task DownloadImageAsync(TPost post, Uri uri)
		{
			try
			{
				using (var resp = await Utils.CreateWebRequest(uri).GetResponseAsync().ConfigureAwait(false))
				{
					if (resp.ContentType.Contains("video/") || resp.ContentType == "image/gif")
					{
						_AnimatedContent.Add(CreateContentLink(post, uri));
						return;
					}
					else if (!resp.ContentType.Contains("image/"))
					{
						Console.WriteLine($"\t{uri} is not an image.");
						return;
					}

					var gottenName = resp.Headers["Content-Disposition"] ?? resp.ResponseUri.LocalPath ?? uri.ToString();
					var cutName = gottenName.Substring(gottenName.LastIndexOf('/') + 1);
					var cleanedName = new string(cutName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
					var file = new FileInfo(Path.Combine(Directory, cleanedName));
					if (file.Exists)
					{
						Console.WriteLine($"\t{file} is already saved.");
						return;
					}

					using (var s = resp.GetResponseStream())
					using (var ms = new MemoryStream())
					{
						//Need to use a memory stream and copy to it
						//Otherwise doing either the md5 hash or creating a bitmap ends up getting to the end of the response stream
						//And with this reponse stream seeks cannot be used on it.
						await s.CopyToAsync(ms).ConfigureAwait(false);

						//A match for the hash has been found, meaning this is a duplicate image
						var hash = ms.Hash<MD5>();
						if (_ImageComparer.TryGetImage(hash, out var alreadyDownloaded))
						{
							Console.WriteLine($"\t{uri} had a matching hash with {alreadyDownloaded.File} meaning they have the same content.");
							return;
						}

						using (var bm = new Bitmap(ms))
						{
							if (bm == default(Bitmap))
							{
								Console.WriteLine($"\t{uri} is unable to be created as a Bitmap and cannot be saved.");
								return;
							}
							else if (bm.PhysicalDimension.Width < MinWidth || bm.PhysicalDimension.Height < MinHeight)
							{
								Console.WriteLine($"\t{uri} is too small.");
								return;
							}

							bm.Save(file.FullName, ImageFormat.Png);
							Console.WriteLine($"\tSaved {uri} to {file}.");

							//Add to list if the download succeeds
							_ImageComparer.TryStore(hash, new ImageDetails(uri, file, bm, _ImageComparer.ThumbnailSize));
						}
					}
				}
			}
			catch (Exception e)
			{
				e.WriteException();
				_FailedDownloads.Add(CreateContentLink(post, uri));
			}
		}
		protected abstract Task<IEnumerable<TPost>> GatherPostsAsync();
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract Task<UriImageGatherer> CreateGatherer(TPost post);
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri);
		/// <summary>
		/// Saves links to a file.
		/// </summary>
		/// <param name="contentLinks">The links to save.</param>
		/// <param name="file">The file to save to.</param>
		public void SaveContentLinks(List<ContentLink> contentLinks, FileInfo file)
		{
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
			var scoreLength = unsavedContent.Max(x => x.Score).GetLength();
			var whatToWrite = String.Join(Environment.NewLine, unsavedContent.Select(x => $"{x.Score.ToString().PadLeft(scoreLength, '0')} {x.Uri}"));
			using (var writer = file.AppendText())
			{
				writer.WriteLine($"{title} - {Utils.FormatDateTimeForSaving()}");
				writer.WriteLine(whatToWrite);
				writer.WriteLine();
			}
			Console.WriteLine($"Added {unsavedContent.Count()} links to {file.Name}.");
		}
	}
}
