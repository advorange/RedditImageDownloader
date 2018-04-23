using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		/// The size of the thumbnail. Bigger = more accurate, but slowness grows at n^2.
		/// </summary>
		protected int ThumbnailSize;
		/// <summary>
		/// The database holding all of the images.
		/// </summary>
		protected LiteDatabase Database;

		/// <summary>
		/// Creates an instance of <see cref="ImageComparer"/>.
		/// </summary>
		/// <param name="databasePath">The path to the database. Cannot stay null.</param>
		/// <param name="thumbnailSize">The width and height to make the thumbnail hashes.</param>
		public ImageComparer(string databasePath, int thumbnailSize = 32)
		{
			databasePath = databasePath ?? throw new ArgumentException("The database directory cannot be null.");
			Database = new LiteDatabase($"filename={databasePath};mode=exclusive;password=123;");
			ThumbnailSize = thumbnailSize;
		}

		/// <inheritdoc />
		public bool TryStore(FileInfo file, Stream stream, int width, int height, out string error)
		{
			var hash = stream.GetMD5Hash();
			var col = Database.GetCollection<ImageDetails>(GetDirectoryTable(file.Directory));
			if (col.FindById(hash) is ImageDetails entry)
			{
				//Only return false if the file still exists
				if (File.Exists(entry.FilePath))
				{
					error = $"had a matching hash with {entry.FilePath}.";
					return false;
				}
				else
				{
					error = null;
					col.Upsert(new ImageDetails(hash, file.FullName, width, height, entry.HashedThumbnail));
					return true;
				}
			}

			try
			{
				error = null;
				stream.Seek(0, SeekOrigin.Begin);
				col.Upsert(new ImageDetails(hash, file.FullName, width, height, GenerateThumbnailHash(stream, ThumbnailSize)));
				return true;
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
			var col = Database.GetCollection<ImageDetails>(GetDirectoryTable(directory));
			col.Delete(x => !File.Exists(x.FilePath)); //Remove all that don't exist anymore
			var alreadyCached = col.FindAll().Select(x => x.FilePath);
			var files = directory.GetFiles().Where(x => x.FullName.IsImagePath() && !alreadyCached.Contains(x.FullName))
				.OrderBy(x => x.CreationTimeUtc)
				.Select((file, index) => new { file, index })
				.GroupBy(x => x.index / imagesPerThread)
				.Select(g => g.Select(obj => obj.file));
			var fileCount = files.Count();
			var count = 0;
			var toDelete = new List<FileInfo>();
			await Task.WhenAll(files.Select(group => Task.Run(() =>
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
							ConsoleUtils.WriteLine($"Certain match between {newEntry} and {entry}. Will delete {newEntry}.");
							toDelete.Add(new FileInfo(newEntry.FilePath));
						}
						else
						{
							col.Upsert(newEntry);
						}
					}
					catch (ArgumentException)
					{
						ConsoleUtils.WriteLine($"{file} is not a valid image.");
					}

					var c = Interlocked.Increment(ref count);
					if (c % 25 == 0 || c == fileCount)
					{
						ConsoleUtils.WriteLine($"{Math.Min(c, fileCount)}/{fileCount} images cached.");
					}
				}
			}, token))).CAF();
			RecyclingUtils.MoveFiles(toDelete.Distinct());
		}
		/// <inheritdoc />
		public void DeleteDuplicates(DirectoryInfo directory, Percentage matchPercentage)
		{
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var col = Database.GetCollection<ImageDetails>(GetDirectoryTable(directory));
			col.Delete(x => !File.Exists(x.FilePath)); //Remove all that don't exist anymore
			var details = new List<ImageDetails>(col.FindAll());
			var count = details.Count;
			var toDelete = new List<FileInfo>();
			for (int i = count - 1; i > 0; --i)
			{
				if (i % 25 == 0 || i == count - 1)
				{
					ConsoleUtils.WriteLine($"{i + 1} image(s) left to check for duplicates.");
				}

				var iVal = details[i];
				for (int j = i - 1; j >= 0; --j)
				{
					var jVal = details[j];
					if (!iVal.Equals(jVal, matchPercentage))
					{
						continue;
					}

					//Check once again but with a higher resolution
					var res = new[] { iVal.Width, iVal.Height, jVal.Width, jVal.Height, 512 }.Min();
					if (!TryCreateImageDetailsFromFile(iVal.FilePath, res, out var newIVal) ||
						!TryCreateImageDetailsFromFile(jVal.FilePath, res, out var newJVal) ||
						!newIVal.Equals(newJVal, matchPercentage))
					{
						continue;
					}

					var delete = (iVal.Width * iVal.Height) < (jVal.Width * jVal.Height) ? iVal : jVal;
					ConsoleUtils.WriteLine($"Certain match between {iVal} and {jVal}. Will delete {delete}.");
					col.Delete(delete.Hash);
					details.Remove(delete);
					toDelete.Add(new FileInfo(delete.FilePath));
				}
			}
			RecyclingUtils.MoveFiles(toDelete.Distinct());
			ConsoleUtils.WriteLine($"{toDelete.Count} match(es) found and deleted.");
		}
		/// <inheritdoc />
		public void Dispose()
		{
			Database?.Dispose();
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
		/// <param name="filePath">The file to cache.</param>
		/// <param name="thumbnailSize">The size to create the thumbnail.</param>
		/// <param name="details">The details of the image.</param>
		/// <returns></returns>
		private bool TryCreateImageDetailsFromFile(string filePath, int thumbnailSize, out ImageDetails details)
		{
			//The second check is because for some reason file.Exists will be true when the file does NOT exist
			if (!filePath.IsImagePath() || !File.Exists(filePath))
			{
				details = default;
				return false;
			}

			using (var stream = File.OpenRead(filePath))
			{
				var (width, height) = stream.GetImageSize();
				stream.Seek(0, SeekOrigin.Begin);
				var hash = stream.GetMD5Hash();
				stream.Seek(0, SeekOrigin.Begin);
				details = new ImageDetails(hash, filePath, width, height, GenerateThumbnailHash(stream, thumbnailSize));
				return true;
			}
		}
		/// <summary>
		/// Puts the name in a valid format.
		/// </summary>
		/// <param name="directory"></param>
		/// <returns></returns>
		private string GetDirectoryTable(DirectoryInfo directory)
		{
			var col = Database.GetCollection<DirectoryCollection>("DirectoryNames");
			if (col.FindById(directory.FullName) is DirectoryCollection entry)
			{
				return entry.Guid.ToString();
			}

			var directoryCollection = new DirectoryCollection(directory);
			col.Insert(directoryCollection);
			return directoryCollection.Guid.ToString();
		}

		/// <summary>
		/// Maps a directory path to a guid 
		/// </summary>
		private sealed class DirectoryCollection
		{
			/// <summary>
			/// The path of the directory.
			/// </summary>
			[BsonId, BsonField("DirectoryPath")]
			public string DirectoryPath { get; set; }
			/// <summary>
			/// The id of the collection.
			/// </summary>
			[BsonField("Guid")]
			public Guid Guid { get; set; }

			/// <summary>
			/// Creates an instance of <see cref="DirectoryCollection"/>.
			/// </summary>
			internal DirectoryCollection()
			{
				DirectoryPath = null;
				Guid = Guid.Empty;
			}
			/// <summary>
			/// Creates an instance of <see cref="DirectoryCollection"/>.
			/// </summary>
			/// <param name="directory"></param>
			internal DirectoryCollection(DirectoryInfo directory)
			{
				DirectoryPath = directory.FullName;
				Guid = Guid.NewGuid();
			}
		}
	}
}