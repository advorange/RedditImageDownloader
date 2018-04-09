﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageComparing
{
	/// <summary>
	/// Compare images so duplicates don't get downloaded or kept.
	/// </summary>
	public abstract class ImageComparer : IImageComparer
	{
		/// <inheritdoc />
		public int StoredImages => Images.Count;
		/// <inheritdoc />
		public int ThumbnailSize { get; set; } = 32;

		/// <summary>
		/// The images which have currently been cached.
		/// </summary>
		protected ConcurrentDictionary<string, ImageDetails> Images = new ConcurrentDictionary<string, ImageDetails>();

		/// <inheritdoc />
		public bool TryStore(Uri url, FileInfo file, Stream stream, int width, int height, out string error)
		{
			var hash = stream.GetMD5Hash();
			if (Images.TryGetValue(hash, out var value))
			{
				error = $"{url} had a matching hash with {value.File}.";
				return false;
			}

			try
			{
				error = null;
				return Images.TryAdd(hash, new ImageDetails(url, file, width, height, GenerateThumbnailHash(stream, ThumbnailSize)));
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}
		}
		/// <inheritdoc />
		public async Task CacheSavedFilesAsync(DirectoryInfo directory, int imagesPerThread, CancellationToken token = default)
		{
			//Don't cache files which have already been cached
			//Accidentally left this not get checked before, which led to me trying to delete 600 files in separate actions
			//Which froze my PC weirdly. My mouse could move/keep hearing the video I was watching, but I had a ~3 minute input lag
			var alreadyCached = Images.Select(x => x.Value.File.FullName);
			var files = directory.GetFiles().Where(x => x.FullName.IsImagePath() && !alreadyCached.Contains(x.FullName))
				.OrderBy(x => x.CreationTimeUtc).ToArray();
			var len = files.Length;
			var grouped = files.Select((file, index) => new { file, index })
				.GroupBy(x => x.index / imagesPerThread)
				.Select(g => g.Select(obj => obj.file));
			var count = 0;
			var tasks = grouped.Select(group => Task.Run(() =>
			{
				foreach (var file in group)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						if (!TryCreateImageDetailsFromFile(file, ThumbnailSize, out var md5hash, out var details))
						{
							ConsoleUtils.WriteLine($"Failed to create a cached object of {file}.");
						}
						//If the file is already in there, delete whichever is worse
						else if (Images.TryGetValue(md5hash, out var alreadyStoredVal))
						{
							details.DetermineWhichToDelete(alreadyStoredVal);
						}
						else if (!Images.TryAdd(md5hash, details))
						{
							ConsoleUtils.WriteLine($"Failed to cache {file}.");
						}
					}
					catch (ArgumentException)
					{
						ConsoleUtils.WriteLine($"{file} is not a valid image.");
					}

					var c = Interlocked.Increment(ref count);
					if (c % 25 == 0 || c == len)
					{
						ConsoleUtils.WriteLine($"{Math.Min(c, len)}/{len} images cached.");
					}
				}
			}, token));
			await Task.WhenAll(tasks).CAF();
		}
		/// <inheritdoc />
		public void DeleteDuplicates(Percentage matchPercentage)
		{
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var kvps = new List<ImageDetails>(Images.Values);
			var kvpCount = kvps.Count;
			var filesToDelete = new List<FileInfo>();
			for (int i = kvpCount - 1; i > 0; --i)
			{
				if (i % 25 == 0 || i == kvpCount - 1)
				{
					ConsoleUtils.WriteLine($"{i + 1} image(s) left to check for duplicates.");
				}

				var iVal = kvps[i];
				for (int j = i - 1; j >= 0; --j)
				{
					var jVal = kvps[j];
					if (!iVal.Equals(jVal, matchPercentage))
					{
						continue;
					}

					//Check once again but with a higher resolution
					var resToCheck = new[] { iVal.Width, iVal.Height, jVal.Width, jVal.Height, 512 }.Min();
					if (!TryCreateImageDetailsFromFile(iVal.File, resToCheck, out var md5Hashi, out var newIVal) ||
						!TryCreateImageDetailsFromFile(jVal.File, resToCheck, out var md5Hashj, out var newJVal) ||
						!newIVal.Equals(newJVal, matchPercentage))
					{
						continue;
					}

					var detailsToDelete = iVal.DetermineWhichToDelete(jVal);
					ConsoleUtils.WriteLine($"Certain match between {iVal.File.Name} and {jVal.File.Name}. Will delete {detailsToDelete.File.Name}.");
					kvps.Remove(detailsToDelete);
					filesToDelete.Add(detailsToDelete.File);
				}
			}

			RecyclingUtils.MoveFiles(filesToDelete.Distinct());
			ConsoleUtils.WriteLine($"{filesToDelete.Count} match(es) found and deleted.");
		}
		/// <summary>
		/// Generates a hash where true = light, false = dark. Used in comparing images for mostly similar instead of exactly similar.
		/// </summary>
		/// <param name="s">The image's data.</param>
		/// <param name="thumbnailSize">The size to make the image.</param>
		/// <returns>The image's hash.</returns>
		protected abstract IEnumerable<bool> GenerateThumbnailHash(Stream s, int thumbnailSize);
		/// <summary>
		/// Attempts to create image details from a file.
		/// </summary>
		/// <param name="file">The file to cache.</param>
		/// <param name="thumbnailSize">The size to create the thumbnail.</param>
		/// <param name="md5Hash">The hash of the image's stream.</param>
		/// <param name="details">The details of the image.</param>
		/// <returns></returns>
		private bool TryCreateImageDetailsFromFile(FileInfo file, int thumbnailSize, out string md5Hash, out ImageDetails details)
		{
			//The second check is because for some reason file.Exists will be true when the file does NOT exist
			if (!file.FullName.IsImagePath() || !File.Exists(file.FullName))
			{
				md5Hash = null;
				details = default;
				return false;
			}

			using (var fs = file.OpenRead())
			{
				md5Hash = fs.GetMD5Hash();
				var (width, height) = fs.GetImageSize();
				details = new ImageDetails(new Uri(file.FullName), file, width, height, GenerateThumbnailHash(fs, thumbnailSize));
				return true;
			}
		}
	}
}