using ImageDL.Utilities;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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
			=> !_Images.ContainsKey(hash) && _Images.TryAdd(hash, details);
		/// <summary>
		/// Returns true if successfully able to get a value with <paramref name="hash"/>;
		/// </summary>
		/// <param name="hash">The hash to search for.</param>
		/// <param name="details">The returned value.</param>
		/// <returns>Returns a boolean indicating whether or not the hash is linked to a value.</returns>
		public bool TryGetImage(string hash, out ImageDetails details)
			=> _Images.TryGetValue(hash, out details);
		/// <summary>
		/// Caches the hashes of each image that is already saved in the directory.
		/// </summary>
		/// <param name="directory"></param>
		public void CacheAlreadySavedFiles(DirectoryInfo directory)
		{
#if DEBUG
			var sw = new Stopwatch();
			sw.Start();
#endif
			var images = directory.GetFiles().Where(x => x.FullName.IsImagePath()).OrderBy(x => x.CreationTimeUtc).ToArray();
			var count = images.Count();
			for (int i = 0; i < count; ++i)
			{
				if (i % 25 == 0 || i == count - 1)
				{
					Console.WriteLine($"{i + 1}/{count} images cached.");
				}

				try
				{
					var img = images[i];
					if (!ImageDetails.TryCreateFromFile(img, ThumbnailSize, out var md5hash, out var details))
					{
						Console.WriteLine($"Failed to cache the already saved file {img}.");
					}
					else if (!TryStore(md5hash, details))
					{
						Console.WriteLine($"Found a duplicate file based on hash {img}. Deleting it.");
						FileSystem.DeleteFile(img.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
					}
				}
				catch (Exception e)
				{
					e.Write();
				}
			}
#if DEBUG
			sw.Stop();
			Console.WriteLine($"Time taken: {sw.ElapsedTicks} ticks, {sw.ElapsedMilliseconds} milliseconds");
#endif
			Console.WriteLine();
		}
		/// <summary>
		/// When the images have finished downloading run through each of them again to see if any are duplicates.
		/// </summary>
		/// <param name="percentForMatch">The percentage of similarity for an image to be considered a match. Ranges from 1 to 100.</param>
		public void DeleteDuplicates(float percentForMatch)
		{
#if DEBUG
			var sw = new Stopwatch();
			sw.Start();
#endif
			Console.WriteLine();
			//Put the kvp values in a separate list so they can be iterated through
			//Start at the top and work the way down
			var kvps = new List<ImageDetails>(_Images.Values);
			var matchCount = 0;
			for (int i = kvps.Count - 1; i > 0; --i)
			{
				if (i % 25 == 0 || i == kvps.Count - 1)
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

					Console.WriteLine($"Possible match between {iVal.File} and {jVal.File}.");

					//Check once again but with a higher resolution
					if (!ImageDetails.TryCreateFromFile(iVal.File, 512, out var md5Hashi, out var newIVal) ||
						!ImageDetails.TryCreateFromFile(jVal.File, 512, out var md5Hashj, out var newJVal) ||
						!newIVal.Equals(newJVal, percentForMatch))
					{
						continue;
					}
					++matchCount;

					//Delete/remove whatever is the smaller image
					var iPixCount = iVal.Width * iVal.Height;
					var jPixCount = jVal.Width * jVal.Height;
					//If each image has the same pixel count then go by file creation time
					//If not then just go by pixel count
					var removeJ = iPixCount == jPixCount
						? iVal.File?.CreationTimeUtc > jVal.File?.CreationTimeUtc
						: iPixCount > jPixCount;
					var fileToDelete = removeJ ? jVal.File : iVal.File;

					kvps.RemoveAt(removeJ ? j : i);
					if (fileToDelete.Exists)
					{
						try
						{
							Console.WriteLine($"Certain match between {iVal.File} and {jVal.File}. Deleting {fileToDelete}.");
							FileSystem.DeleteFile(fileToDelete.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
						}
						catch
						{
							Console.WriteLine($"Unable to delete the duplicate image {fileToDelete}.");
						}
					}
				}
				++CurrentImagesSearched;
			}
			Console.WriteLine($"{matchCount} match(es) found.");
#if DEBUG
			sw.Stop();
			Console.WriteLine($"Time taken: {sw.ElapsedTicks} ticks, {sw.ElapsedMilliseconds} milliseconds");
#endif
			Console.WriteLine();
			//Clear the lists and dictionary so after all this is done the program uses less memory
			kvps.Clear();
			_Images.Clear();
		}
		private void NotifyPropertyChanged([CallerMemberName] string name = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
