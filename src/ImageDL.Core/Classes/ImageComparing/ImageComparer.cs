using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

using LiteDB;

namespace ImageDL.Classes.ImageComparing
{
	/// <summary>
	/// Compare images so duplicates don't get downloaded or kept.
	/// </summary>
	public abstract class ImageComparer : IImageComparer, IDisposable
	{
		/// <summary>
		/// The database holding all of the images.
		/// </summary>
		protected LiteDatabase Database;

		/// <summary>
		/// The size of the thumbnail. Bigger = more accurate, but slowness grows at n^2.
		/// </summary>
		protected int ThumbnailSize;

		/// <summary>
		/// Creates an instance of <see cref="ImageComparer"/>.
		/// </summary>
		/// <param name="databasePath">The path to the database. Cannot stay null.</param>
		/// <param name="thumbnailSize">The width and height to make the thumbnail hashes.</param>
		protected ImageComparer(string databasePath, int thumbnailSize = 32)
		{
			databasePath = databasePath ?? throw new ArgumentException("The database directory cannot be null.");
			Database = new LiteDatabase(new ConnectionString
			{
				Filename = databasePath,
				Mode = LiteDB.FileMode.Exclusive,
			});
			ThumbnailSize = thumbnailSize;
		}

		/// <inheritdoc />
		public async Task<int> CacheSavedFilesAsync(DirectoryInfo directory, int imagesPerThread, CancellationToken token = default)
		{
			//Don't cache files which have already been cached
			//Accidentally left this not get checked before, which led to me trying to delete 600 files in separate actions
			//Which froze my PC weirdly. My mouse could move/keep hearing the video I was watching, but I had a ~3 minute input lag
			var col = Database.GetCollection<ImageDetails>(/*GetDirectoryCollectionName(directory)*/);
			col.Delete(x => !File.Exists(Path.Combine(directory.FullName, x.FileName))); //Remove all that don't exist anymore
			var alreadyCached = col.FindAll().Select(x => x.FileName).ToList();
			var files = directory.GetFiles()
				.Where(x => x.FullName.IsImagePath() && !alreadyCached.Contains(x.Name))
				.OrderBy(x => x.CreationTimeUtc);
			var fileCount = files.Count();
			var count = 0;
			var successCount = 0;
			var duplicates = new List<FileInfo>();
			await Task.WhenAll(files.GroupInto(imagesPerThread).Select(group => Task.Run(() =>
			{
				foreach (var file in group)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						if (!TryCreateImageDetailsFromFile(file.FullName, ThumbnailSize, out var newEntry))
						{
							ConsoleUtils.WriteLine($"Failed to create a cached object of {file}.");
						}
						else if (col.FindById(newEntry.Hash) is ImageDetails entry) //Delete new entry
						{
							ConsoleUtils.WriteLine($"Certain match between {newEntry} and {entry}. Will remove {newEntry}.");
							duplicates.Add(new FileInfo(Path.Combine(directory.FullName, newEntry.FileName)));
						}
						else
						{
							col.Upsert(newEntry);
							Interlocked.Increment(ref successCount);
						}
					}
					catch (ArgumentException)
					{
						ConsoleUtils.WriteLine($"{file} is not a valid image.");
					}

					var c = Interlocked.Increment(ref count);
					if (c % 25 == 0 || c == fileCount)
					{
						ConsoleUtils.WriteLine($"{c}/{fileCount} images cached.");
					}
				}
			}, token))).CAF();
			RemoveDuplicates(directory, duplicates.Distinct());
			return successCount;
		}

		/// <inheritdoc />
		public void Dispose()
			=> Database?.Dispose();

		/// <inheritdoc />
		public int HandleDuplicates(DirectoryInfo directory, Percentage matchPercentage)
		{
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var col = Database.GetCollection<ImageDetails>(/*GetDirectoryCollectionName(directory)*/);
			col.Delete(x => !File.Exists(Path.Combine(directory.FullName, x.FileName))); //Remove all that don't exist anymore
			var details = new List<ImageDetails>(col.FindAll().ToList());
			var count = details.Count;
			var duplicates = new List<FileInfo>();
			for (var i = count - 1; i > 0; --i)
			{
				if (i % 25 == 0 || i == count - 1)
				{
					ConsoleUtils.WriteLine($"{i + 1} image(s) left to check for duplicates.");
				}

				var iVal = details[i];
				for (var j = i - 1; j >= 0; --j)
				{
					var jVal = details[j];
					if (!iVal.Equals(jVal, matchPercentage))
					{
						continue;
					}

					//Check once again but with a higher resolution
					var res = new[] { iVal.Width, iVal.Height, jVal.Width, jVal.Height, 512 }.Min();
					if (!TryCreateImageDetailsFromFile(Path.Combine(directory.FullName, iVal.FileName), res, out var newIVal)
						|| !TryCreateImageDetailsFromFile(Path.Combine(directory.FullName, jVal.FileName), res, out var newJVal)
						|| !newIVal.Equals(newJVal, matchPercentage))
					{
						continue;
					}

					var delete = (iVal.Width * iVal.Height) < (jVal.Width * jVal.Height) ? iVal : jVal;
					ConsoleUtils.WriteLine($"Certain match between {iVal} and {jVal}. Will remove {delete}.");
					col.Delete(delete.Hash);
					details.Remove(delete);
					duplicates.Add(new FileInfo(Path.Combine(directory.FullName, delete.FileName)));
					--i;
				}
			}
			RemoveDuplicates(directory, duplicates.Distinct());
			return duplicates.Count;
		}

		/// <inheritdoc />
		public bool TryStore(FileInfo file, Stream stream, int width, int height, out string error)
		{
			var hash = stream.GetMD5Hash();
			var col = Database.GetCollection<ImageDetails>(/*GetDirectoryCollectionName(file.Directory)*/);
			if (col.FindById(hash) is ImageDetails entry)
			{
				//Only return false if the file still exists
				if (File.Exists(Path.Combine(file.DirectoryName, entry.FileName)))
				{
					error = $"had a matching hash with {entry.FileName}.";
					return false;
				}
				else
				{
					error = null;
					col.Upsert(new ImageDetails(hash, file.Name, width, height, entry.HashedThumbnail));
					return true;
				}
			}

			try
			{
				error = null;
				stream.Seek(0, SeekOrigin.Begin);
				var hash2 = HashImageStream(stream, ThumbnailSize);
				col.Upsert(new ImageDetails(hash, file.Name, width, height, hash2));
				return true;
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}
		}

		/// <summary>
		/// Averages out the brightnesses then converts the values into 1s and 0s based on if they are greater than or less than the average.
		/// </summary>
		/// <param name="brightnesses"></param>
		/// <returns></returns>
		protected virtual string BrightnessesToString(float[] brightnesses)
		{
			var avgBrightness = brightnesses.Average();
			var values = new char[brightnesses.Length];
			for (var i = 0; i < brightnesses.Length; ++i)
			{
				values[i] = brightnesses[i] > avgBrightness ? '1' : '0';
			}
			return new string(values);
		}

		/// <summary>
		/// Calculates a single float value for the specified ARGB values representing the brightness of the pixel
		/// </summary>
		/// <param name="a"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected float CalculateBrightness(byte a, byte r, byte g, byte b)
			//Magic numbers for caclulating brightness, see: https://stackoverflow.com/a/596243
			=> ((0.299f * r) + (0.587f * g) + (0.114f * b)) * (a / 255f);

		/// <summary>
		/// Generates a hash of an image. Used in comparing images for mostly similar instead of exactly similar.
		/// </summary>
		/// <param name="s">The image's data.</param>
		/// <param name="size">The size to make the image.</param>
		/// <returns>The image's hash.</returns>
		protected abstract string HashImageStream(Stream s, int size);

		/// <summary>
		/// Removes the duplicates from the current folder.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="files"></param>
		protected virtual void RemoveDuplicates(DirectoryInfo directory, IEnumerable<FileInfo> files)
		{
			if (!files.Any())
			{
				return;
			}

			var duplicateDirectory = Path.Combine(directory.FullName, "Duplicates");
			if (!Directory.Exists(duplicateDirectory))
			{
				Directory.CreateDirectory(duplicateDirectory);
			}

			foreach (var file in files)
			{
				if (!file.Exists)
				{
					continue;
				}

				var newPath = Path.Combine(duplicateDirectory, file.Name);
				if (File.Exists(newPath))
				{
					for (var i = 1; i < int.MaxValue; ++i)
					{
						var newPathName = Path.GetFileNameWithoutExtension(newPath);
						var newPathExt = Path.GetExtension(newPath);
						var incrementedPath = $"{newPathName} ({i}){newPathExt}";
						if (!File.Exists(incrementedPath))
						{
							newPath = incrementedPath;
							break;
						}
					}
				}

				try
				{
					file.MoveTo(newPath);
				}
				catch (FileNotFoundException) { }
			}
		}

		/// <summary>
		/// Attempts to create image details from a file.
		/// </summary>
		/// <param name="filePath">The file to cache.</param>
		/// <param name="size">The size to make the thumbnails when comparing.</param>
		/// <param name="details">The details of the image.</param>
		/// <returns></returns>
		private bool TryCreateImageDetailsFromFile(string filePath, int size, out ImageDetails details)
		{
			//The second check is because for some reason file.Exists will be true when the file does NOT exist
			if (!filePath.IsImagePath() || !File.Exists(filePath))
			{
				details = default;
				return false;
			}

			using var stream = File.OpenRead(filePath);

			//TODO: see if this can be done in a single call to the stream
			var (width, height) = stream.GetImageSize();
			stream.Seek(0, SeekOrigin.Begin);
			var hash = stream.GetMD5Hash();
			stream.Seek(0, SeekOrigin.Begin);
			var hash2 = HashImageStream(stream, size);
			details = new ImageDetails(hash, Path.GetFileName(filePath), width, height, hash2);
			return true;
		}
	}
}