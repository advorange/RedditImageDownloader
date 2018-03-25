using AdvorangesUtils;
using ImageDL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageComparers
{
	/// <summary>
	/// Compare images so duplicates don't get downloaded or kept.
	/// </summary>
	public abstract class ImageComparer : IImageComparer
	{
		/// <inheritdoc/>
		public int StoredImages => Images.Count;
		/// <inheritdoc/>
		public int CurrentImagesSearched
		{
			get => _CurrentImagesSearched;
			private set
			{
				_CurrentImagesSearched = value;
				NotifyPropertyChanged();
			}
		}
		/// <inheritdoc/>
		public int ThumbnailSize
		{
			get => _ThumbnailSize;
			set => _ThumbnailSize = value;
		}

		/// <summary>
		/// The images which have currently been cached.
		/// </summary>
		protected ConcurrentDictionary<string, ImageDetails> Images = new ConcurrentDictionary<string, ImageDetails>();

		private int _CurrentImagesSearched;
		private int _ThumbnailSize = 32;

		/// <summary>
		/// Indicates when <see cref="CurrentImagesSearched"/> is incremented.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public bool TryStore(Uri uri, FileInfo file, Stream stream, int width, int height, out string error)
		{
			var hash = stream.GetMD5Hash();
			if (Images.TryGetValue(hash, out var value))
			{
				error = $"{uri} had a matching hash with {value.File}.";
				return false;
			}

			try
			{
				error = null;
				return Images.TryAdd(hash, new ImageDetails(uri, file, width, height, GenerateThumbnailHash(stream, ThumbnailSize)));
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}
		}
		/// <inheritdoc />
		public bool TryCreateImageDetailsFromFile(FileInfo file, int thumbnailSize, out string md5Hash, out ImageDetails details)
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
							Console.WriteLine($"Failed to create a cached object of {file}.");
						}
						//If the file is already in there, delete whichever is worse
						else if (Images.TryGetValue(md5hash, out var alreadyStoredVal))
						{
							details.DetermineWhichToDelete(alreadyStoredVal);
						}
						else if (!Images.TryAdd(md5hash, details))
						{
							Console.WriteLine($"Failed to cache {file}.");
						}
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
			}, token));
			await Task.WhenAll(tasks).CAF();
		}
		/// <inheritdoc />
		public void DeleteDuplicates(float percentForMatch)
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
					if (!TryCreateImageDetailsFromFile(iVal.File, resToCheck, out var md5Hashi, out var newIVal) ||
						!TryCreateImageDetailsFromFile(jVal.File, resToCheck, out var md5Hashj, out var newJVal) ||
						!newIVal.Equals(newJVal, percentForMatch))
					{
						continue;
					}

					var detailsToDelete = iVal.DetermineWhichToDelete(jVal);
					Console.WriteLine($"Certain match between {iVal.File.Name} and {jVal.File.Name}. Will delete {detailsToDelete.File.Name}.");
					kvps.Remove(detailsToDelete);
					filesToDelete.Add(detailsToDelete.File);
				}
				++CurrentImagesSearched;
			}

			RecyclingUtils.MoveFiles(filesToDelete);
			Console.WriteLine($"{filesToDelete.Count} match(es) found and deleted.");
		}
		/// <inheritdoc />
		public abstract IEnumerable<bool> GenerateThumbnailHash(Stream s, int thumbnailSize);
		/// <summary>
		/// Invokes <see cref="PropertyChanged"/>.
		/// </summary>
		/// <param name="name"></param>
		protected void NotifyPropertyChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
