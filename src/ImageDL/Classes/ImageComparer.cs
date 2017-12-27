using ImageDL.Utilities;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
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
		private float _MaxAcceptableImageSimilarity;
		public float MaxAcceptableImageSimilarity
		{
			get => _MaxAcceptableImageSimilarity;
			set => _MaxAcceptableImageSimilarity = value;
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
			var images = directory.GetFiles();
			for (int i = 0; i < images.Count(); ++i)
			{
				if (i % 25 == 0 || i == images.Count())
				{
					Console.WriteLine($"{i}/{images.Count()} images cached.");
				}

				try
				{
					if (!ImageDetails.TryCreateFromFile(images[i], ThumbnailSize, out var md5hash, out var details) || !TryStore(md5hash, details))
					{
						Console.WriteLine($"Failed to cache the already saved file '{images[i]}'");
					}
				}
				catch (Exception e)
				{
					e.WriteException();
					continue;
				}
			}
			Console.WriteLine();
		}
		/// <summary>
		/// When the images have finished downloading run through each of them again to see if any are duplicates.
		/// </summary>
		public void DeleteDuplicates(float percentForMatch)
		{
			//Put the kvp values in a separate list so they can be iterated through
			var kvps = _Images.ToList();
			//Start at the top and work the way down
			for (int i = kvps.Count - 1; i > 0; --i)
			{
				var iVal = kvps[i].Value;
				for (int j = i - 1; j >= 0; --j)
				{
					var jVal = kvps[j].Value;
					if (!iVal.Equals(jVal, percentForMatch))
					{
						continue;
					}

					//Check once again but with a higher resolution
					ImageDetails.TryCreateFromFile(iVal.File, 512, out var md5Hashi, out var newIVal);
					ImageDetails.TryCreateFromFile(jVal.File, 512, out var md5Hashj, out var newJVal);
					if (!newIVal.Equals(newJVal, percentForMatch))
					{
						continue;
					}

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
							fileToDelete.Delete();
						}
						catch
						{
							Console.WriteLine($"Unable to delete the duplicate image '{fileToDelete}'.");
						}
					}
					break;
				}
				++CurrentImagesSearched;
			}
			//Clear the lists and dictionary so after all this is done the program uses less memory
			kvps.Clear();
			_Images.Clear();
		}
		private void NotifyPropertyChanged([CallerMemberName] string name = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
