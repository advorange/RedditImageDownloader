using ImageDL.Classes;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TPost">The type of each post. Some might be uris, some might be specified classes.</typeparam>
	public abstract class GenericImageDownloader<TPost> : ImageDownloader
	{
		public GenericImageDownloader() : base() { }

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
				Console.WriteLine("Unable to find any images matching the search criteria.");
				return;
			}

			_ImageComparer = new ImageComparer { ThumbnailSize = 32, };
			if (CompareSavedImages)
			{
				Console.WriteLine();
				await _ImageComparer.CacheSavedFilesAsync(new DirectoryInfo(Directory), ImagesCachedPerThread);
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
						_Links.Add(CreateContentLink(post, imageUri, "Failed Download"));
					}
				}
			}
			BusyDownloading = false;

			Console.WriteLine();
			_ImageComparer.DeleteDuplicates(MaxImageSimilarity / 1000f);
			Console.WriteLine();
			DownloadsFinished = true;

			SaveStoredContentLinks();
		}
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="post">The post to save from.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		public async Task<string> DownloadImageAsync(UriImageGatherer gatherer, TPost post, Uri uri)
		{
			if (!String.IsNullOrWhiteSpace(gatherer.Error))
			{
				return gatherer.Error;
			}
			else if (gatherer.IsVideo)
			{
				_Links.Add(CreateContentLink(post, gatherer.OriginalUri, "Animated Content"));
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
					_Links.Add(CreateContentLink(post, uri, "Animated Content"));
					return $"{uri} is animated content (gif/video).";
				}
				if (!resp.ContentType.Contains("image/"))
				{
					return $"{uri} is not an image.";
				}
				var file = new FileInfo(Path.Combine(Directory, GenerateFileName(post, resp, uri)));
				if (File.Exists(file.FullName))
				{
					return $"{uri} is already saved as {file}.";
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating a bitmap ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = resp.GetResponseStream()).CopyToAsync(ms = new MemoryStream()).ConfigureAwait(false);
				var hash = ms.Hash<MD5>();
				if (_ImageComparer.TryGetImage(hash, out var alreadyDownloaded))
				{
					//A match for the hash has been found, meaning this is a duplicate 
					return $"{uri} had a matching hash with {alreadyDownloaded.File}.";
				}

				var details = new ImageDetails(uri, file, ms, _ImageComparer.ThumbnailSize);
				if (details.Width < MinWidth || details.Height < MinHeight)
				{
					return $"{uri} is too small ({details.Width}x{details.Height}).";
				}
				if (_ImageComparer.TryStore(hash, details))
				{
					//Add to list if the download succeeds
					await ms.CopyToAsync(fs = file.Create()).ConfigureAwait(false);
					return $"Saved {uri} to {file}.";
				}
				return $"Failed to save {uri} to {file}.";
			}
			finally
			{
				resp?.Dispose();
				rs?.Dispose();
				ms?.Dispose();
				fs?.Dispose();
			}
		}

		protected abstract Task<List<TPost>> GatherPostsAsync();
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract string GenerateFileName(TPost post, WebResponse response, Uri uri);
		protected abstract Task<UriImageGatherer> CreateGathererAsync(TPost post);
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri, string reason);
	}
}
