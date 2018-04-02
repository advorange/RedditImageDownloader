﻿using AdvorangesUtils;
using ImageDL.Classes.ImageComparing;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private static readonly string NL = Environment.NewLine;
		private static readonly string NLT = NL + "\t";

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
			}
		}
		/// <inheritdoc />
		public int AmountOfPostsToGather
		{
			get => _AmountOfPostsToGather;
			set => _AmountOfPostsToGather = Math.Max(1, value);
		}
		/// <inheritdoc />
		public int MinWidth
		{
			get => _MinWidth;
			set => _MinWidth = Math.Max(0, value);
		}
		/// <inheritdoc />
		public int MinHeight
		{
			get => _MinHeight;
			set => _MinHeight = Math.Max(0, value);
		}
		/// <inheritdoc />
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set => _MaxDaysOld = Math.Max(0, value);
		}
		/// <inheritdoc />
		public Percentage MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set => _MaxImageSimilarity = value;
		}
		/// <inheritdoc />
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set => _ImagesCachedPerThread = Math.Max(1, value);
		}
		/// <inheritdoc />
		public int MinScore
		{
			get => _MinScore;
			set => _MinScore = Math.Max(0, value);
		}
		/// <inheritdoc />
		public AspectRatio MinAspectRatio
		{
			get => _MinAspectRatio;
			set => _MinAspectRatio = value;
		}
		/// <inheritdoc />
		public AspectRatio MaxAspectRatio
		{
			get => _MaxAspectRatio;
			set => _MaxAspectRatio = value;
		}
		/// <inheritdoc />
		public bool CompareSavedImages
		{
			get => _CompareSavedImages;
			set => _CompareSavedImages = value;
		}
		/// <inheritdoc />
		public bool CreateDirectory
		{
			get => _CreateDirectory;
			set => _CreateDirectory = value;
		}
		/// <inheritdoc />
		public bool Start
		{
			get => _Start;
			set => _Start = value;
		}
		/// <inheritdoc />
		public ImageComparer ImageComparer
		{
			get => _ImageComparer;
			set => _ImageComparer = value;
		}
		/// <inheritdoc />
		public SettingParser SettingParser
		{
			get => _SettingParser;
			set => _SettingParser = value;
		}
		/// <inheritdoc />
		public DateTime OldestAllowed => DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
		/// <inheritdoc />
		public bool CanStart => Start && SettingParser.AllSet;
		/// <inheritdoc />
		public string Name => _Name;

		/// <summary>
		/// Links to content that is animated, failed to download, etc.
		/// </summary>
		protected List<ContentLink> Links = new List<ContentLink>();
		/// <summary>
		/// Used to download files.
		/// </summary>
		protected ImageDownloaderClient Client;
		/// <summary>
		/// To make sure only one instance is running at a time.
		/// </summary>
		protected SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

		private string _Directory;
		private int _AmountOfPostsToGather;
		private int _MinWidth;
		private int _MinHeight;
		private int _MaxDaysOld;
		private Percentage _MaxImageSimilarity;
		private int _ImagesCachedPerThread;
		private int _MinScore;
		private AspectRatio _MinAspectRatio;
		private AspectRatio _MaxAspectRatio;
		private bool _CompareSavedImages;
		private bool _CreateDirectory;
		private bool _Start;
		private ImageComparer _ImageComparer;
		private SettingParser _SettingParser;
		private string _Name;

		/// <summary>
		/// Creates an image downloader.
		/// </summary>
		/// <param name="name">The name of the website.</param>
		public ImageDownloader(string name)
		{
			SettingParser = new SettingParser(new[] { "--", "-", "/" })
			{
				new Setting<string>(new[] { nameof(Directory), "dir" }, x => Directory = x)
				{
					Description = "The directory to save to.",
				},
				new Setting<int>(new[] {nameof(AmountOfPostsToGather), "amt" }, x => AmountOfPostsToGather = x)
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
				new Setting<Percentage>(new[] {nameof(MaxImageSimilarity), "sim" }, x => MaxImageSimilarity = x, Percentage.TryParse)
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
				new Setting<AspectRatio>(new[] {nameof(MinAspectRatio), "minar" }, x => MinAspectRatio = x, AspectRatio.TryParse)
				{
					Description = "The minimum aspect ratio for an image to have before being ignored.",
					DefaultValue = new AspectRatio(0, 1),
				},
				new Setting<AspectRatio>(new[] {nameof(MaxAspectRatio), "maxar" }, x => MaxAspectRatio = x, AspectRatio.TryParse)
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
				new Setting<bool>(new[] {nameof(Start), "s" }, x => Start = x)
				{
					Description = "Whether or not to start the downloader. Will not start until all other arguments are provided.",
					IsFlag = true,
				},
			};
			Client = new ImageDownloaderClient(new CookieContainer());
			_Name = name;

			//Save on close in case program is closed while running
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => SaveStoredContentLinks();
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
		}

		/// <inheritdoc />
		public async Task StartAsync(CancellationToken token = default)
		{
			await SemaphoreSlim.WaitAsync(token).CAF();
			Console.WriteLine();

			var posts = new List<TPost>();
			try
			{
				await GatherPostsAsync(posts).CAF();
			}
			catch (Exception e)
			{
				e.Write();
			}

			//Make sure some posts were gotten.
			if (!posts.Any())
			{
				Console.WriteLine("Unable to find any posts matching the search criteria.");
				return;
			}
			else
			{
				posts = OrderAndRemoveDuplicates(posts);
				Console.WriteLine();
				Console.WriteLine($"Found {posts.Count} posts.");
			}

			var count = 0;
			foreach (var post in posts)
			{
				token.ThrowIfCancellationRequested();
				WritePostToConsole(post, ++count);

				var gatherer = await GatherImagesAsync(post).CAF();
				//If the gatherer had any errors simply log them once and then be done with it
				if (!String.IsNullOrWhiteSpace(gatherer.Error))
				{
					if (gatherer.IsAnimated)
					{
						Links.AddRange(gatherer.ImageUris.Select(x => CreateContentLink(post, x, ANIMATED_CONTENT)));
					}
					WriteInColor($"\t{gatherer.Error.Replace(NL, NLT)}", ConsoleColor.Yellow);
					continue;
				}

				for (int i = 0; i < gatherer.ImageUris.Length; ++i)
				{
					var imageUri = gatherer.ImageUris[i];
					try
					{
						var (Response, IsSuccess) = await DownloadImageAsync(gatherer, post, imageUri).CAF();
						var text = $"\t[#{i + 1}] {Response.Replace(NL, NLT)}";
						WriteInColor(text, IsSuccess ? Console.ForegroundColor : ConsoleColor.Yellow);
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

			Console.WriteLine($"Added {SaveStoredContentLinks()} links to file.");
			SemaphoreSlim.Release();
		}
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="result">The gathered image uris.</param>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		protected async Task<(string Response, bool IsSuccess)> DownloadImageAsync(ScrapeResult result, TPost post, Uri uri)
		{
			var file = GenerateFileInfo(post, uri);
			if (File.Exists(file.FullName))
			{
				return ($"{uri} is already saved as {file}.", false);
			}

			HttpResponseMessage resp = null;
			Stream rs = null;
			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				resp = await Client.SendWithRefererAsync(uri, HttpMethod.Get).CAF();
				if (!resp.IsSuccessStatusCode)
				{
					Links.Add(CreateContentLink(post, result.OriginalUri, $"HTTP error: {resp.StatusCode.ToString()}"));
					return ($"{uri} received the error: {resp.ToString()}", false);
				}
				var contentType = resp.Content.Headers.GetValues("Content-Type").First();
				if (contentType.Contains("video/") || contentType == "image/gif")
				{
					Links.Add(CreateContentLink(post, uri, ANIMATED_CONTENT));
					return ($"{uri} is animated content (gif/video).", false);
				}
				if (!contentType.Contains("image/"))
				{
					return ($"{uri} is not an image.", false);
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating an image ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = await resp.Content.ReadAsStreamAsync().CAF()).CopyToAsync(ms = new MemoryStream());

				//If image is too small, don't bother saving
				var (width, height) = ms.GetImageSize();
				if (!FitsSizeRequirements(uri, width, height, out var sizeError))
				{
					return (sizeError, false);
				}
				//If the image comparer returns any errors when trying to store, then return that error
				if (ImageComparer != null && !ImageComparer.TryStore(uri, file, ms, width, height, out var cachingError))
				{
					return (cachingError, false);
				}

				//Save the file
				ms.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs = file.Create()).CAF();
				return ($"Saved {uri} to {file}.", true);
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
		/// Returns a file info with a maximum path length of 255 characters.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="name"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		protected FileInfo GenerateFileInfo(string directory, string name, string extension)
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
		/// <summary>
		/// Saves the stored content links to file.
		/// Returns the amount of links put into the file.
		/// </summary>
		protected int SaveStoredContentLinks()
		{
			var file = new FileInfo(Path.Combine(Directory, "Links.txt"));
			var links = Links.GroupBy(x => x.Uri).Select(x => x.First()).ToList();
			//Only read once, make sure no duplicate uris will be added
			if (file.Exists)
			{
				using (var reader = new StreamReader(file.OpenRead()))
				{
					var text = reader.ReadToEnd();
					for (int i = links.Count - 1; i >= 0; --i)
					{
						if (text.Contains(links[i].Uri.ToString()))
						{
							links.RemoveAt(i);
						}
					}
				}
			}
			//Put all of the links with the same reasons together, ordered by score
			var groups = links.GroupBy(x => x.Reason).Select(g =>
			{
				var len = g.Max(x => x.AssociatedNumber).ToString().Length;
				var format = g.OrderByDescending(x => x.AssociatedNumber)
					.Select(x => $"{x.AssociatedNumber.ToString().PadLeft(len, '0')} {x.Uri}");
				return $"{g.Key.FormatTitle()} - {Formatting.ToSaving()}{NL}{String.Join(NL, format)}{NL}";
			});
			//Only write for a short time
			using (var writer = file.AppendText())
			{
				foreach (var line in groups)
				{
					writer.WriteLine(line);
				}
			}
			return links.Count;
		}
		/// <summary>
		/// Adds the object to the list, prints to the console if its a multiple of 25, and returns true if still allowed to download more.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		protected bool Add(List<TPost> list, TPost obj)
		{
			list.Add(obj);
			if (list.Count % 25 == 0)
			{
				Console.WriteLine($"{list.Count} {Name} posts found.");
			}
			return list.Count < AmountOfPostsToGather;
		}
		private void WriteInColor(string text, ConsoleColor color)
		{
			if (Console.ForegroundColor == color)
			{
				Console.WriteLine(text);
			}
			else
			{
				var oldColor = Console.ForegroundColor;
				Console.ForegroundColor = color;
				Console.WriteLine(text);
				Console.ForegroundColor = oldColor;
			}
		}

		/// <summary>
		/// Gathers the posts which match the supplied settings.
		/// </summary>
		/// <param name="list">The list to add values to.</param>
		/// <returns></returns>
		protected abstract Task GatherPostsAsync(List<TPost> list);
		/// <summary>
		/// Reorders the list and removes duplicate entries.
		/// </summary>
		/// <param name="list"></param>
		/// <returns>The ordered list.</returns>
		protected abstract List<TPost> OrderAndRemoveDuplicates(List<TPost> list);
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