using ImageDL.Core.Utilities;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using MetadataExtractor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageComparers
{
	/// <summary>
	/// Compare images so duplicates don't get downloaded or kept.
	/// </summary>
	/// <typeparam name="T"></typeparam>
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

		private int _CurrentImagesSearched;
		private int _ThumbnailSize = 32;
		protected ConcurrentDictionary<string, ImageDetails> Images = new ConcurrentDictionary<string, ImageDetails>();

		/// <summary>
		/// Indicates when <see cref="CurrentImagesSearched"/> is incremented.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets the width and height of an image through metadata.
		/// </summary>
		/// <param name="s">The image's data.</param>
		/// <returns></returns>
		public static (int Width, int Height) GetImageSize(Stream s)
		{
			try
			{
				s.Seek(0, SeekOrigin.Begin);
				var metadata = ImageMetadataReader.ReadMetadata(s);
				var tags = metadata.SelectMany(x => x.Tags);
				var width = Convert.ToInt32(tags.Single(x => x.Name == "Image Width").Description.Split(' ')[0]);
				var height = Convert.ToInt32(tags.Single(x => x.Name == "Image Height").Description.Split(' ')[0]);
				return (width, height);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Unable to parse the image width and height from the file's metadata.", e);
			}
		}
		/// <inheritdoc/>
		public bool TryStore(Uri uri, FileInfo file, Stream stream, int minWidth, int minHeight, out string error)
		{
			var hash = stream.MD5Hash();
			if (Images.TryGetValue(hash, out var value))
			{
				error = $"{uri} had a matching hash with {value.File}.";
				return false;
			}

			try
			{
				var (width, height) = GetImageSize(stream);
				if (width < minWidth || height < minHeight)
				{
					error = $"{uri} is too small ({width}x{height}).";
					return false;
				}

				error = null;
				return Images.TryAdd(hash, new ImageDetails(uri, file, width, height, GenerateThumbnailHash(stream, ThumbnailSize)));
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}
		}
		/// <inheritdoc/>
		public async Task CacheSavedFilesAsync(DirectoryInfo directory, int imagesPerThread)
		{
			var files = directory.GetFiles().Where(x => x.FullName.IsImagePath()).OrderBy(x => x.CreationTimeUtc).ToArray();
			var len = files.Length;
			var grouped = files.Select((file, index) => new { file, index })
				.GroupBy(x => x.index / imagesPerThread)
				.Select(g => g.Select(obj => obj.file));
			var count = 0;
			var tasks = grouped.Select(group => Task.Run(() =>
			{
				foreach (var file in group)
				{
					try
					{
						if (!TryCreateImageDetailsFromFile(file, ThumbnailSize, out var md5hash, out var details))
						{
							Console.WriteLine($"Failed to create a cached object of {file}.");
						}
						//If the file is already in there, delete whichever is worse
						else if (Images.TryGetValue(md5hash, out var alreadyStoredVal))
						{
							Delete(details, alreadyStoredVal);
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
			}));
			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
		/// <inheritdoc/>
		public void DeleteDuplicates(float percentForMatch)
		{
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var kvps = new List<ImageDetails>(Images.Values);
			var kvpCount = kvps.Count;
			var deleteCount = 0;
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
					else if (Delete(iVal, jVal) is ImageDetails deletedDetails)
					{
						kvps.Remove(deletedDetails);
						++deleteCount;
					}
				}
				++CurrentImagesSearched;
			}
			Console.WriteLine($"{deleteCount} match(es) found and deleted.");
		}
		/// <inheritdoc/>
		public abstract IEnumerable<bool> GenerateThumbnailHash(Stream s, int thumbnailSize);
		/// <summary>
		/// Attempts to create image details from a file.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="thumbnailSize"></param>
		/// <param name="md5Hash"></param>
		/// <param name="details"></param>
		/// <returns></returns>
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
				md5Hash = fs.MD5Hash();
				var (width, height) = GetImageSize(fs);
				details = new ImageDetails(new Uri(file.FullName), file, width, height, GenerateThumbnailHash(fs, ThumbnailSize));
				return true;
			}
		}
		/// <summary>
		/// Attempts to delete one of the image details.
		/// </summary>
		/// <param name="i1"></param>
		/// <param name="i2"></param>
		/// <param name="deletedDetails"></param>
		/// <returns></returns>
		public ImageDetails Delete(ImageDetails i1, ImageDetails i2)
		{
			//Delete/remove whatever is the smaller image
			var firstPix = i1.Width * i1.Height;
			var secondPix = i2.Width * i2.Height;
			//If each image has the same pixel count then go by file creation time, if not then just go by pixel count
			var removeFirst = firstPix == secondPix ? i1.File?.CreationTimeUtc < i2.File?.CreationTimeUtc : firstPix < secondPix;
			var deletedDetails = removeFirst ? i1 : i2;
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					RecycleBinMover.MoveFile(deletedDetails.File);
				}
				else
				{
					deletedDetails.File.Delete();
				}

				Console.WriteLine($"Certain match between {i1.File.Name} and {i2.File.Name}. Deleted {deletedDetails.File.Name}.");
				return deletedDetails;
			}
			catch
			{
				Console.WriteLine($"Unable to delete the duplicate image {deletedDetails.File}.");
				return deletedDetails;
			}
		}
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
