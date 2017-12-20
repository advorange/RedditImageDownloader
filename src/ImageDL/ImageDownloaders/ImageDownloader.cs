using ImageDL.Classes;
using ImageDL.Enums;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
		};

		protected List<ContentLink> _AnimatedContent = new List<ContentLink>();
		protected List<ContentLink> _FailedDownloads = new List<ContentLink>();
		protected PropertyInfo[] _Arguments = new PropertyInfo[0];
		protected PropertyInfo[] _UnsetArguments => _Arguments.Where(x => !_SetArguments.Contains(x)).ToArray();
		protected List<PropertyInfo> _SetArguments = new List<PropertyInfo>();

		public bool IsReady
		{
			get
			{
				if (!String.IsNullOrWhiteSpace(Directory) && !System.IO.Directory.Exists(Directory))
				{
					Console.WriteLine($"{Directory} does not exist as a directory.");
					return false;
				}
				return !_UnsetArguments.Any();
			}
		}
		private string _Directory;
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
		public int MinHeight
		{
			get => _MinHeight;
			set
			{
				_MinHeight = value;
				AddArgumentToSetArguments();
			}
		}

		public event EventHandler AllArgumentsSet;
		public event EventHandler DownloadsFinished;

		public ImageDownloader(params string[] args)
		{
			_Arguments = GetSettings(GetType());
			if (args.Any())
			{
				AddArguments(args);
			}
		}

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		public async Task StartAsync()
		{
			var count = 0;
			foreach (var post in await GatherPostsAsync())
			{
				//TODO: should this be left in?
				await Task.Delay(25);
				WritePostToConsole(post, ++count);
				foreach (var imageUri in GatherImages(post))
				{
					switch (UriUtils.CorrectUri(imageUri, out var correctedUri))
					{
						case UriCorrectionResponse.Valid:
						case UriCorrectionResponse.Unknown:
						{
							await DownloadImageAsync(post, correctedUri);
							continue;
						}
						case UriCorrectionResponse.Animated:
						{
							_AnimatedContent.Add(CreateContentLink(post, correctedUri));
							continue;
						}
						case UriCorrectionResponse.Invalid:
						{
							continue;
						}
					}
				}
			}
			SaveAnimatedContent(new DirectoryInfo(Directory));
			SaveFailedDownloads(new DirectoryInfo(Directory));
			DownloadsFinished?.Invoke(this, new EventArgs());
		}
		protected abstract Task<IEnumerable<TPost>> GatherPostsAsync();
		protected abstract IEnumerable<Uri> GatherImages(TPost post);
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri);

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
				var req = (HttpWebRequest)WebRequest.Create(uri);
				req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
				req.Timeout = 5000;
				req.ReadWriteTimeout = 5000;
				using (var resp = await req.GetResponseAsync())
				{
					var gottenName = (resp.Headers["Content-Disposition"] ?? resp.ResponseUri.LocalPath ?? uri.ToString().Split('/').Last());
					var cutName = gottenName.Substring(gottenName.LastIndexOf('/') + 1);
					var cleanedName = new string(cutName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());

					var savePath = Path.Combine(Directory, cleanedName);
					if (!resp.ContentType.Contains("image"))
					{
						Console.WriteLine($"\t{uri} is not an image.");
						return;
					}
					else if (File.Exists(savePath))
					{
						Console.WriteLine($"\t{savePath} is already saved.");
						return;
					}

					using (var s = resp.GetResponseStream())
					using (var bm = new Bitmap(s))
					{
						//TODO: compare hashes?
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

						//TODO: async save?
						bm.Save(savePath, ImageFormat.Png);
						Console.WriteLine($"\tSaved {uri} to {savePath}.");
					}
				}
			}
			catch (Exception e)
			{
				e.WriteException();
				_FailedDownloads.Add(CreateContentLink(post, uri));
			}
		}
		/// <summary>
		/// Saves all the links gotten to animated content.
		/// </summary>
		/// <param name="directory">The folder to save to.</param>
		protected void SaveAnimatedContent(DirectoryInfo directory)
		{
			if (_AnimatedContent.Any())
			{
				SaveContentLinks(_AnimatedContent, new FileInfo(Path.Combine(directory.FullName, "Animated_Content.txt")));
			}
		}
		/// <summary>
		/// Saves all the links that failed to download.
		/// </summary>
		/// <param name="directory">The folder to save to.</param>
		protected void SaveFailedDownloads(DirectoryInfo directory)
		{
			if (_FailedDownloads.Any())
			{
				SaveContentLinks(_AnimatedContent, new FileInfo(Path.Combine(directory.FullName, "Failed_Downloads.txt")));
			}
		}
		/// <summary>
		/// Saves links to a file.
		/// </summary>
		/// <param name="contentLinks">The links to save.</param>
		/// <param name="file">The file to save to.</param>
		protected void SaveContentLinks(List<ContentLink> contentLinks, FileInfo file)
		{
			//Only save links which are not already in the text document
			var unsavedContent = new List<ContentLink>();
			if (file.Exists)
			{
				using (var reader = new StreamReader(file.OpenRead()))
				{
					var text = reader.ReadToEnd();
					foreach (var anim in _AnimatedContent)
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
			var scoreLength = unsavedContent.Max(x => x.Score).GetLengthOfNumber();
			var whatToWrite = String.Join(Environment.NewLine, unsavedContent.Select(x => $"{x.Score.ToString().PadLeft(scoreLength, '0')} {x.Uri}"));
			using (var writer = file.AppendText())
			{
				writer.WriteLine($"{title} - {Utils.FormatDateTimeForSaving()}");
				writer.WriteLine(whatToWrite);
				writer.WriteLine();
			}
			Console.WriteLine($"Added {unsavedContent.Count()} links to {file.Name}.");
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

				//If number then use the tryparses, if string just set, if neither then nothing
				if (String.IsNullOrWhiteSpace(split[1]))
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
			else if (!_UnsetArguments.Any())
			{
				AllArgumentsSet?.Invoke(this, new EventArgs());
			}
		}
		/// <summary>
		/// Prints out to the console what arguments are still needed.
		/// </summary>
		public void AskForArguments()
		{
			if (!_UnsetArguments.Any())
			{
				return;
			}

			var sb = new StringBuilder("The following arguments need to be set:" + Environment.NewLine);
			foreach (var argument in _UnsetArguments)
			{
				sb.AppendLine($"\t{argument.Name} ({argument.PropertyType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}
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
		public static PropertyInfo[] GetSettings(Type type)
			=> type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string)) && x.SetMethod != null)
				.OrderByNonComparable(x => x.PropertyType)
				.ToArray();
	}
}
