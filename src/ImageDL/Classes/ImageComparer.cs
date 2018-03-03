//#define SINGLE_SYNC
//#define PARALLEL
//#define FOREACH_ASYNC
#define GROUPED_ASYNC

using ImageDL.Utilities;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	/// <summary>
	/// Attempts to get rid of duplicate images.
	/// </summary>
	public sealed class ImageComparer : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public int StoredImages => _Images.Count;
		private int _CurrentImagesSearched;
		public int CurrentImagesSearched
		{
			get => _CurrentImagesSearched;
			private set
			{
				_CurrentImagesSearched = value;
				NotifyPropertyChanged();
			}
		}
		private int _ThumbnailSize;
		public int ThumbnailSize
		{
			get => _ThumbnailSize;
			set => _ThumbnailSize = value;
		}

		private ConcurrentDictionary<string, ImageDetails> _Images = new ConcurrentDictionary<string, ImageDetails>();

		/// <summary>
		/// Returns false if this was not able to be added to the image comparer's dictionary or is already added.
		/// </summary>
		/// <param name="details">The image's details.</param>
		/// <returns>Returns a boolean indicating whether or not the image details were successfully stored.</returns>
		public bool TryStore(string hash, ImageDetails details)
		{
			return _Images.TryAdd(hash, details);
		}
		/// <summary>
		/// Returns true if successfully able to get a value with <paramref name="hash"/>;
		/// </summary>
		/// <param name="hash">The hash to search for.</param>
		/// <param name="details">The returned value.</param>
		/// <returns>Returns a boolean indicating whether or not the hash is linked to a value.</returns>
		public bool TryGetImage(string hash, out ImageDetails details)
		{
			return _Images.TryGetValue(hash, out details);
		}
		/// <summary>
		/// Caches every file in the directory.
		/// </summary>
		/// <param name="directory">The directory to cache.</param>
		public async Task CacheSavedFiles(DirectoryInfo directory, int taskGroupLength = 50)
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
#endif
			var files = directory.GetFiles().Where(x => x.FullName.IsImagePath()).OrderBy(x => x.CreationTimeUtc).ToArray();
			var len = files.Length;
#if SINGLE_SYNC //At least 4x slower than Grouped_Async. Very low memory usage
			for (int i = 0; i < len; ++i)
			{
				if (i % 25 == 0 || i == len - 1)
				{
					Console.WriteLine($"{i + 1}/{len} images cached.");
				}

				var file = files[i];
				try
				{
					CacheFile(file);
				}
				catch (ArgumentException)
				{
					Console.WriteLine($"{file} is not a valid image.");
				}
			}
#elif PARALLEL //Extremely high CPU usage, very fast.
			Parallel.ForEach(files, file =>
			{
				try
				{
					CacheFile(file);
				}
				catch (ArgumentException)
				{
					Console.WriteLine($"{file} is not a valid image.");
				}
			});
#elif FOREACH_ASYNC //Effectively same as Parallel
			var tasks = new List<Task>();
			for (int i = 0; i < len; ++i)
			{
				if (i % 25 == 0 || i == len - 1)
				{
					Console.WriteLine($"{i + 1}/{len} images cached.");
				}

				var file = files[i];
				tasks.Add(Task.Run(() =>
				{
					try
					{
						CacheFile(file);
					}
					catch (ArgumentException)
					{
						Console.WriteLine($"{file} is not a valid image.");
					}
				}));
			}
			await Task.WhenAll(tasks).ConfigureAwait(false);
#elif GROUPED_ASYNC //Best combination of speed and memory usage.
			var grouped = files.Select((file, index) => new { file, index })
				.GroupBy(x => x.index / taskGroupLength)
				.Select(g => g.Select(obj => obj.file));
			var count = 0;
			var tasks = grouped.Select(group => Task.Run(() =>
			{
				foreach (var file in group)
				{
					try
					{
						CacheFile(file);
					}
					catch (ArgumentException)
					{
						Console.WriteLine($"{file} is not a valid image.");
					}

					var c = Interlocked.Increment(ref count);
					if (c % 25 == 0 || c == len)
					{
						Console.WriteLine($"{Math.Min(c, len)}/{len} images cached.");
					}
				}
			}));
			await Task.WhenAll(tasks).ConfigureAwait(false);
#endif
#if DEBUG
			sw.Stop();
			Console.WriteLine($"Time taken: {sw.ElapsedTicks} ticks, {sw.ElapsedMilliseconds} milliseconds");
#endif
		}
		/// <summary>
		/// Caches the file.
		/// </summary>
		/// <param name="file">The file to cache.</param>
		public void CacheFile(FileInfo file)
		{
			if (!ImageDetails.TryCreateFromFile(file, ThumbnailSize, out var md5hash, out var details))
			{
				Console.WriteLine($"Failed to create a cached object of {file}.");
			}
			//If the file is already in there, delete whichever is worse
			else if (_Images.TryGetValue(md5hash, out var alreadyStoredVal))
			{
				TryDelete(details, alreadyStoredVal, out var deletedDetails);
			}
			else if (!TryStore(md5hash, details))
			{
				Console.WriteLine($"Failed to cache {file}.");
			}
		}
		/// <summary>
		/// When the images have finished downloading run through each of them again to see if any are duplicates.
		/// </summary>
		/// <param name="percentForMatch">The percentage of similarity for an image to be considered a match. Ranges from 1 to 100.</param>
		public void DeleteDuplicates(float percentForMatch)
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
#endif
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var kvps = new List<ImageDetails>(_Images.Values);
			var kvpCount = kvps.Count;
			var matchCount = 0;
			for (int i = kvpCount - 1; i > 0; --i)
			{
				if (i % 25 == 0 || i == kvpCount - 1)
				{
					Console.WriteLine($"{i} image(s) left to check for duplicates.");
				}

				var iVal = kvps[i];
				for (int j = i - 1; j >= 0; --j)
				{
					var jVal = kvps[j];
					if (!iVal.Equals(jVal, percentForMatch))
					{
						continue;
					}

					//Check once again but with a higher resolution
					var resToCheck = new[] { iVal.Width, iVal.Height, jVal.Width, jVal.Height, 512 }.Min();
					if (!ImageDetails.TryCreateFromFile(iVal.File, resToCheck, out var md5Hashi, out var newIVal) ||
						!ImageDetails.TryCreateFromFile(jVal.File, resToCheck, out var md5Hashj, out var newJVal) ||
						!newIVal.Equals(newJVal, percentForMatch))
					{
						continue;
					}
					else if (TryDelete(iVal, jVal, out var deletedDetails))
					{
						kvps.Remove(deletedDetails);
						Console.WriteLine($"Certain match between {iVal.File.Name} and {jVal.File.Name}. Deleting {deletedDetails.File.Name}.");
					}
					else
					{
						Console.WriteLine($"Unable to delete the duplicate image {deletedDetails.File}.");
					}
					++matchCount;
				}
				++CurrentImagesSearched;
			}
			Console.WriteLine($"{matchCount} match(es) found.");
#if DEBUG
			sw.Stop();
			Console.WriteLine($"Time taken: {sw.ElapsedTicks} ticks, {sw.ElapsedMilliseconds} milliseconds");
#endif
		}
		/// <summary>
		/// Deletes a file. Returns true if <paramref name="i1"/> is deleted, false if <paramref name="i2"/> is deleted.
		/// </summary>
		/// <param name="i1">The first file to potentially delete.</param>
		/// <param name="i2">The second file to potentially delete.</param>
		/// <param name="deletedDetails">The details which the file associated with them has been deleted.</param>
		private bool TryDelete(ImageDetails i1, ImageDetails i2, out ImageDetails deletedDetails)
		{
			//Delete/remove whatever is the smaller image
			var firstPix = i1.Width * i1.Height;
			var secondPix = i2.Width * i2.Height;
			//If each image has the same pixel count then go by file creation time
			//If not then just go by pixel count
			var removeFirst = firstPix == secondPix
				? i1.File?.CreationTimeUtc < i2.File?.CreationTimeUtc
				: firstPix < secondPix;
			deletedDetails = removeFirst ? i1 : i2;
			try
			{
				FileSystem.DeleteFile(deletedDetails.File.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
