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

			_ImageComparer = new ImageComparer { ThumbnailSize = 32, };
			if (CompareSavedImages)
			{
				Console.WriteLine();
				await _ImageComparer.CacheSavedFiles(new DirectoryInfo(Directory), ImagesCachedPerThread);
				Console.WriteLine();
			}

			var count = 0;
			foreach (var post in (await GatherPostsAsync().ConfigureAwait(false)).Take(AmountToDownload))
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
						_FailedDownloads.Add(CreateContentLink(post, imageUri));
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
				_AnimatedContent.Add(CreateContentLink(post, gatherer.OriginalUri));
				return $"{gatherer.OriginalUri} is animated content (gif/video).";
			}

			WebResponse resp = null;
			Stream s = null;
			MemoryStream ms = null;
			Bitmap bm = null;
			try
			{
				resp = await uri.CreateWebRequest().GetResponseAsync().ConfigureAwait(false);
				if (resp.ContentType.Contains("video/") || resp.ContentType == "image/gif")
				{
					_AnimatedContent.Add(CreateContentLink(post, uri));
					return $"{uri} is animated content (gif/video).";
				}
				else if (!resp.ContentType.Contains("image/"))
				{
					return $"{uri} is not an image.";
				}

				var file = new FileInfo(Path.Combine(Directory, GenerateFileName(post, resp, uri)));
				if (File.Exists(file.FullName))
				{
					return $"{file} is already saved.";
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating a bitmap ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (s = resp.GetResponseStream()).CopyToAsync(ms = new MemoryStream()).ConfigureAwait(false);

				//A match for the hash has been found, meaning this is a duplicate image
				var hash = ms.Hash<MD5>();
				if (_ImageComparer.TryGetImage(hash, out var alreadyDownloaded))
				{
					return $"{uri} had a matching hash with {alreadyDownloaded.File} meaning they have the same content.";
				}
				else if ((bm = new Bitmap(ms)) == default(Bitmap))
				{
					return $"{uri} is the default bitmap and cannot be saved.";
				}
				else if (bm.PhysicalDimension.Width < MinWidth || bm.PhysicalDimension.Height < MinHeight)
				{
					return $"{uri} is too small.";
				}

				bm.Save(file.FullName, ImageFormat.Png);
				//Add to list if the download succeeds
				_ImageComparer.TryStore(hash, new ImageDetails(uri, file, ms, _ImageComparer.ThumbnailSize));
				return $"Saved {uri} to {file}.";
			}
			finally
			{
				resp?.Dispose();
				s?.Dispose();
				ms?.Dispose();
				bm?.Dispose();
			}
		}

		protected abstract Task<IEnumerable<TPost>> GatherPostsAsync();
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract string GenerateFileName(TPost post, WebResponse response, Uri uri);
		protected abstract Task<UriImageGatherer> CreateGathererAsync(TPost post);
		protected abstract ContentLink CreateContentLink(TPost post, Uri uri);
	}
}
