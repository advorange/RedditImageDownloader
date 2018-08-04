using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	[DownloaderName("Unknown")]
	public abstract class PostDownloader : IPostDownloader, IPostGatherer, IHasSettings
	{
		private static readonly string NL = Environment.NewLine;
		private static readonly string NLT = NL + "\t";

		/// <summary>
		/// The path of the directory to save images to.
		/// </summary>
		public string SavePath
		{
			get => _SavePath;
			set
			{
				if (!_CreateDirectory && !System.IO.Directory.Exists(value))
				{
					throw new ArgumentException($"{value} is not already created and -cd has not been used.", nameof(SavePath));
				}
				var dir = System.IO.Directory.CreateDirectory(value);
				_SavePath = dir.FullName ?? throw new ArgumentException($"{value} is an invalid directory name.", nameof(SavePath));
			}
		}
		/// <summary>
		/// The amount of posts to look through.
		/// </summary>
		public int AmountOfPostsToGather
		{
			get => _AmountOfPostsToGather;
			set => _AmountOfPostsToGather = Math.Max(1, value);
		}
		/// <summary>
		/// The minimum width an image can have before it won't be downloaded.
		/// </summary>
		public int MinWidth
		{
			get => _MinWidth;
			set => _MinWidth = Math.Max(0, value);
		}
		/// <summary>
		/// The minimum height an image can have before it won't be downloaded.
		/// </summary>
		public int MinHeight
		{
			get => _MinHeight;
			set => _MinHeight = Math.Max(0, value);
		}
		/// <summary>
		/// The maximum age an image can have before it won't be downloaded.
		/// </summary>
		public int MaxDaysOld
		{
			get => _MaxDaysOld;
			set => _MaxDaysOld = Math.Max(0, value);
		}
		/// <summary>
		/// The maximum allowed image similarity before an image is considered a duplicate.
		/// </summary>
		public Percentage MaxImageSimilarity
		{
			get => _MaxImageSimilarity;
			set => _MaxImageSimilarity = value;
		}
		/// <summary>
		/// How many images to cache per thread. Lower = faster, but more CPU.
		/// </summary>
		public int ImagesCachedPerThread
		{
			get => _ImagesCachedPerThread;
			set => _ImagesCachedPerThread = Math.Max(1, value);
		}
		/// <summary>
		/// The minimum score an image can have before it won't be downloaded. Not every site uses this.
		/// </summary>
		public int MinScore
		{
			get => _MinScore;
			set => _MinScore = Math.Max(0, value);
		}
		/// <summary>
		/// The minimum aspect ratio an image can have.
		/// </summary>
		public AspectRatio MinAspectRatio
		{
			get => _MinAspectRatio;
			set => _MinAspectRatio = value;
		}
		/// <summary>
		/// The maximum aspect ratio an image can have.
		/// </summary>
		public AspectRatio MaxAspectRatio
		{
			get => _MaxAspectRatio;
			set => _MaxAspectRatio = value;
		}
		/// <summary>
		/// Indicates whether or not to create the directory if it does not exist.
		/// </summary>
		public bool CreateDirectory
		{
			get => _CreateDirectory;
			set => _CreateDirectory = value;
		}
		/// <summary>
		/// Indicates the user wants the downloader to start.
		/// </summary>
		public bool Start
		{
			get => _Start;
			set => _Start = value;
		}
		/// <summary>
		/// The datetime of the oldest allowed posts. Is simply <see cref="DateTime.UtcNow"/> minus the amount of days.
		/// </summary>
		public DateTime OldestAllowed => DateTime.UtcNow.Subtract(TimeSpan.FromDays(MaxDaysOld));
		/// <summary>
		/// The directory to save images to.
		/// </summary>
		public DirectoryInfo Directory => new DirectoryInfo(SavePath);
		/// <inheritdoc />
		public SettingParser SettingParser => _SettingParser;
		/// <inheritdoc />
		public bool CanStart => Start && SettingParser.AllSet;

		private string _SavePath;
		private int _AmountOfPostsToGather;
		private int _MinWidth;
		private int _MinHeight;
		private int _MaxDaysOld;
		private Percentage _MaxImageSimilarity;
		private int _ImagesCachedPerThread;
		private int _MinScore;
		private AspectRatio _MinAspectRatio;
		private AspectRatio _MaxAspectRatio;
		private bool _CreateDirectory;
		private bool _Start;
		private readonly SettingParser _SettingParser;

		/// <summary>
		/// Creates an instance of <see cref="PostDownloader"/>.
		/// </summary>
		public PostDownloader()
		{
			_SettingParser = new SettingParser
			{
				new Setting<string>(new[] { nameof(SavePath), "path", "dir" }, x => SavePath = x)
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
				new Setting<Percentage>(new[] {nameof(MaxImageSimilarity), "sim" }, x => MaxImageSimilarity = x,
					s => (Percentage.TryParse(s, out var result), result))
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
				new Setting<AspectRatio>(new[] {nameof(MinAspectRatio), "minar" }, x => MinAspectRatio = x,
					s => (AspectRatio.TryParse(s, out var result), result))
				{
					Description = "The minimum aspect ratio for an image to have before being ignored.",
					DefaultValue = new AspectRatio(0, 1),
				},
				new Setting<AspectRatio>(new[] {nameof(MaxAspectRatio), "maxar" }, x => MaxAspectRatio = x, 
					s => (AspectRatio.TryParse(s, out var result), result))
				{
					Description = "The maximum aspect ratio for an image to have before being ignored.",
					DefaultValue = new AspectRatio(1, 0),
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
		}

		/// <inheritdoc />
		public async Task<DownloaderResponse> DownloadAsync(List<IPost> posts, IServiceProvider services, CancellationToken token = default)
		{
			if (!posts.Any())
			{
				return DownloaderResponse.FromNoPostsFound();
			}

			//This cannot be null, a client is necessary for downloading
			var client = services.GetRequiredService<IDownloaderClient>();
			//This can be null, image comparing is not critical
			var comparer = services.GetService<IImageComparerFactory>()?.CreateComparer(Path.Combine(SavePath, ".ImageDL.db"));

			try
			{
				var downloadedCount = 0;
				var links = new List<ContentLink>();
				for (int i = 0; i < posts.Count; ++i)
				{
					var post = posts[i];
					token.ThrowIfCancellationRequested();
					ConsoleUtils.WriteLine(post.Format(i + 1));

					var images = await post.GetImagesAsync(client).CAF();
					//If wasn't success, log it
					if (images.IsSuccess == false)
					{
						links.AddRange(images.ImageUrls.Select(x => post.CreateContentLink(x, images)));
						ConsoleUtils.WriteLine($"\t{images.Text.Replace(NL, NLT)}", ConsoleColor.Yellow);
						continue;
					}

					//Put the images into groups of 5
					var count = 0;
					foreach (var group in images.ImageUrls.GroupInto(5))
					{
						//Download every image in that group of 5 at the same time to speed up downloading
						await Task.WhenAll(group.Select(async x =>
						{
							Response result;
							try
							{
								result = await DownloadImageAsync(client, comparer, post, x).CAF();
							}
							//Catch all so they can be written and logged as a failed download
							catch (Exception e)
							{
								e.Write();
								links.Add(post.CreateContentLink(x, new Response(ImageResponse.EXCEPTION, e.Message, false)));
								return;
							}

							var text = $"\t[#{Interlocked.Increment(ref count)}] {result.Text.Replace(NL, NLT)}";
							if (result.IsSuccess == true)
							{
								ConsoleUtils.WriteLine(text);
								Interlocked.Increment(ref downloadedCount);
							}
							else if (result.IsSuccess == false)
							{
								ConsoleUtils.WriteLine(text, ConsoleColor.Yellow);
								links.Add(post.CreateContentLink(x, result));
							}
							else
							{
								ConsoleUtils.WriteLine(text, ConsoleColor.Cyan);
							}
						})).CAF();
					}
				}
				ConsoleUtils.WriteLine("");

				var cachedCount = 0;
				var deletedCount = 0;
				if (comparer != null)
				{
					cachedCount = await comparer.CacheSavedFilesAsync(Directory, ImagesCachedPerThread, token).CAF();
					if (cachedCount != 0)
					{
						ConsoleUtils.WriteLine($"{cachedCount} image(s) successfully cached from file.{NL}");
					}
					deletedCount = comparer.HandleDuplicates(Directory, MaxImageSimilarity);
					if (deletedCount != 0)
					{
						ConsoleUtils.WriteLine($"{deletedCount} match(es) found and deleted.{NL}");
					}
				}
				var linkCount = 0;
				if (links.Any())
				{
					linkCount = SaveStoredContentLinks(Directory, links);
					if (linkCount != 0)
					{
						ConsoleUtils.WriteLine($"{linkCount} link(s) added to file.{NL}");
					}
				}
				return DownloaderResponse.FromFinished(posts.Count, downloadedCount, cachedCount, deletedCount, linkCount);
			}
			finally
			{
				if (comparer is IDisposable disposableComparer)
				{
					disposableComparer.Dispose();
				}
			}
		}
		/// <inheritdoc />
		public async Task<List<IPost>> GatherAsync(IServiceProvider services, CancellationToken token = default)
		{
			var client = services.GetRequiredService<IDownloaderClient>(); //Cannot be null, 100% necessary for gathering

			var posts = new List<IPost>();
			try
			{
				await GatherAsync(client, posts, token).CAF();
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				e.Write();
			}

			var sorted = posts.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Score).ToList();
			ConsoleUtils.WriteLine($"Found {sorted.Count} posts.{NL}");
			return sorted;
		}
		/// <summary>
		/// Gathers and downloads posts in one.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DownloaderResponse> DownloadAsync(IServiceProvider services, CancellationToken token = default)
		{
			return await DownloadAsync(await GatherAsync(services, token).CAF(), services, token).CAF();
		}
		/// <summary>
		/// Actual implementation of <see cref="GatherAsync(IServiceProvider, CancellationToken)"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="list"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		protected abstract Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token);
		/// <summary>
		/// Checks min width, min height, and the min/max aspect ratios.
		/// </summary>
		/// <param name="size"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected bool HasValidSize(ISize size, out string error)
		{
			return HasValidSize(size.Width, size.Height, out error);
		}
		/// <summary>
		/// Checks min width, min height, and the min/max aspect ratios.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected bool HasValidSize(int width, int height, out string error)
		{
			if (width < MinWidth || height < MinHeight)
			{
				error = $"is too small ({width}x{height}).";
				return false;
			}
			var aspectRatio = width / (double)height;
			if (aspectRatio < MinAspectRatio.Value || aspectRatio > MaxAspectRatio.Value)
			{
				error = $"does not fit in the aspect ratio restrictions ({width}x{height}).";
				return false;
			}
			error = null;
			return true;
		}
		/// <summary>
		/// Downloads an image from <paramref name="url"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="client">The client to download with.</param>
		/// <param name="comparer">The comparer to use.</param>
		/// <param name="post">The post to save from.</param>
		/// <param name="url">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		protected async Task<Response> DownloadImageAsync(IDownloaderClient client, IImageComparer comparer, IPost post, Uri url)
		{
			var file = post.GenerateFileInfo(Directory, url);
			if (File.Exists(file.FullName))
			{
				return new Response("Already Saved", $"{url} is already saved as {file}.", false);
			}

			HttpResponseMessage resp = null;
			Stream rs = null;
			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				resp = await client.SendAsync(client.GenerateReq(url)).CAF();
				switch (DetermineIfValid(url, resp))
				{
					case ValidityType.Fail:
						return new Response(ImageResponse.EXCEPTION, $"{url} had the following exception:{NL}{resp}", false);
					case ValidityType.Animated:
						return new Response(ImageResponse.ANIMATED, $"{url} is either a video or a gif.", false);
					case ValidityType.NotImage:
						return new Response("Not An Image", $"{url} did not lead to an image.", false);
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating an image ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = await resp.Content.ReadAsStreamAsync().CAF()).CopyToAsync(ms = new MemoryStream());

				//If image is too small, don't bother saving
				var width = -1;
				var height = -1;
				if (post is ISize size)
				{
					width = size.Width;
					height = size.Height;
				}
				else
				{
					ms.Seek(0, SeekOrigin.Begin);
					var (w, h) = ms.GetImageSize();
					width = w;
					height = h;
				}
				if (!HasValidSize(width, height, out var sizeError))
				{
					return new Response("Does Not Meet Size Reqs", $"{url} {sizeError}", false);
				}
				//If the image comparer returns any errors when trying to store, then return that error
				ms.Seek(0, SeekOrigin.Begin);
				if (comparer != null && !comparer.TryStore(file, ms, width, height, out var cachingError))
				{
					return new Response("Unable To Cache", $"{url} {cachingError}", false);
				}
				//Save the file
				ms.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs = file.Create()).CAF();
				return new Response(null, $"Successfully saved {url} to {file}.", true);
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
		/// Saves the stored content links to file.
		/// Returns the amount of links put into the file.
		/// <paramref name="diretory"></paramref>
		/// <paramref name="links"></paramref>
		/// </summary>
		protected int SaveStoredContentLinks(DirectoryInfo diretory, List<ContentLink> links)
		{
			var file = new FileInfo(Path.Combine(diretory.FullName, "Links.txt"));
			var filteredLinks = links.GroupBy(x => x.Url).Select(x => x.First()).ToList();
			//Only read once, make sure no duplicate uris will be added
			if (file.Exists)
			{
				using (var reader = new StreamReader(file.OpenRead()))
				{
					var text = reader.ReadToEnd();
					for (int i = filteredLinks.Count - 1; i >= 0; --i)
					{
						if (text.Contains(filteredLinks[i].Url.ToString()))
						{
							filteredLinks.RemoveAt(i);
						}
					}
				}
			}
			//Put all of the links with the same reasons together, ordered by score
			var groups = filteredLinks.GroupBy(x => x.Reason).Select(g =>
			{
				var len = g.Max(x => x.AssociatedNumber).ToString().Length;
				var format = g.OrderByDescending(x => x.AssociatedNumber)
					.Select(x => $"{x.AssociatedNumber.ToString().PadLeft(len, '0')} {x.Url}");
				return $"{g.Key.ToString().FormatTitle()} - {Formatting.ToSaving()}{NL}{String.Join(NL, format)}{NL}";
			});
			using (var writer = file.AppendText())
			{
				foreach (var line in groups)
				{
					writer.WriteLine(line);
				}
			}
			return filteredLinks.Count;
		}
		/// <summary>
		/// Adds the object to the list, prints to the console if its a multiple of 25, and returns true if still allowed to download more.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="post"></param>
		/// <returns></returns>
		protected bool Add(List<IPost> list, IPost post)
		{
			//Return true to signify keep collecting, but don't add it because duplicate.
			if (list.Any(x => x.Id == post.Id))
			{
				return true;
			}
			list.Add(post);
			if (list.Count % 25 == 0)
			{
				var name = GetType().GetCustomAttribute<DownloaderNameAttribute>()?.Name;
				ConsoleUtils.WriteLine($"{list.Count}{(name != null ? $" {name}" : "")} posts found.");
			}
			return list.Count < AmountOfPostsToGather;
		}
		/// <summary>
		/// Determines if the data is a valid image to download.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="resp"></param>
		/// <returns></returns>
		private ValidityType DetermineIfValid(Uri url, HttpResponseMessage resp)
		{
			if (!resp.IsSuccessStatusCode)
			{
				return ValidityType.Fail;
			}
			var ct = resp.Content.Headers.GetValues("Content-Type").First();
			if (ct.Contains("video/") || ct == "image/gif")
			{
				return ValidityType.Animated;
			}
			//If the content type is image, then we know it's an image
			if (ct.Contains("image/"))
			{
				return ValidityType.Image;
			}
			//If the content type is octet-stream then we need to check the url path and assume its extension is correct
			if (ct == "application/octet-stream" && url.ToString().IsImagePath())
			{
				return ValidityType.Image;
			}
			//If the content type is force download then we need to check the content disposition
			if (ct == "application/force-download" && resp.Content.Headers.TryGetValues("Content-Disposition", out var cd)
				&& new ContentDisposition(cd.Single()).FileName.IsImagePath())
			{
				return ValidityType.Image;
			}
			return ValidityType.NotImage;
		}

		private enum ValidityType
		{
			Image,
			NotImage,
			Animated,
			Fail,
		}
	}
}