using ImageDL.Classes.ImageGatherers;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TPost">The type of each post. Some might be uris, some might be specified classes.</typeparam>
	/// <typeparam name="TImageDetails">The type of image details.</typeparam>
	public abstract class GenericImageDownloader<TPost> : ImageDownloader
	{
		private const int MAX_FILE_PATH_LENGTH = 255;
		private const string ANIMATED_CONTENT = "Animated Content";
		private const string FAILED_DOWNLOADS = "Failed Downloads";

		public GenericImageDownloader(IImageComparer imageComparer) : base(imageComparer) { }

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		/// <returns>An awaitable task which downloads images.</returns>
		public override async Task StartAsync()
		{
			AllArgumentsSet = false;
			BusyDownloading = true;

			var posts = await GatherPostsAsync().ConfigureAwait(false);
			if (!posts.Any())
			{
				Console.WriteLine("Unable to find any posts matching the search criteria.");
				return;
			}

			if (ImageComparer != null && CompareSavedImages)
			{
				Console.WriteLine();
				await ImageComparer.CacheSavedFilesAsync(new DirectoryInfo(Directory), ImagesCachedPerThread);
				Console.WriteLine();
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
					catch (WebException e)
					{
						e.Write();
						Links.Add(CreateContentLink(post, imageUri, FAILED_DOWNLOADS));
					}
				}
			}
			BusyDownloading = false;

			if (ImageComparer != null)
			{
				Console.WriteLine();
				ImageComparer.DeleteDuplicates(MaxImageSimilarity / 1000f);
				Console.WriteLine();
			}
			DownloadsFinished = true;

			SaveStoredContentLinks();
		}
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		public async Task<string> DownloadImageAsync(ImageGatherer gatherer, TPost post, Uri uri)
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
			Image img = null;
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

				var fileName = Path.Combine(Directory, GenerateFileName(post, resp, uri));
				if (fileName.Length > MAX_FILE_PATH_LENGTH)
				{
					throw new InvalidOperationException($"file path + file name may not be longer than {MAX_FILE_PATH_LENGTH} characters in total.");
				}
				var file = new FileInfo(fileName);
				if (File.Exists(file.FullName))
				{
					return $"{uri} is already saved as {file}.";
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating a bitmap ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = resp.GetResponseStream()).CopyToAsync(ms = new MemoryStream()).ConfigureAwait(false);

				//TODO: parse size differently
				img = Image.FromStream(ms, false, false);
				if (img.Width < MinWidth || img.Height < MinHeight)
				{
					return $"{uri} is too small ({img.Width}x{img.Height}).";
				}

				if (ImageComparer == null || ImageComparer.TryStore(uri, file, ms, out var error))
				{
					//Add to list if the download succeeds
					ms.Seek(0, SeekOrigin.Begin);
					await ms.CopyToAsync(fs = file.Create()).ConfigureAwait(false);
					return $"Saved {uri} to {file}.";
				}
				else
				{
					return error;
				}
			}
			finally
			{
				resp?.Dispose();
				rs?.Dispose();
				ms?.Dispose();
				img?.Dispose();
				fs?.Dispose();
			}
		}

		protected abstract Task<List<TPost>> GatherPostsAsync();
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract string GenerateFileName(TPost post, WebResponse response, Uri uri);
		protected abstract Task<ImageGatherer> CreateGathererAsync(TPost post);
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri, string reason);
	}
}
