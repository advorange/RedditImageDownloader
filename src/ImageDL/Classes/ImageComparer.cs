using ImageDL.ImageDownloaders;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ImageDL.Classes
{
	/// <summary>
	/// Attempts to get rid of duplicate images.
	/// </summary>
	public class ImageComparer
	{
		private ConcurrentDictionary<string, ImageDetails> _Images = new ConcurrentDictionary<string, ImageDetails>();
		public double PercentForMatch;

		public ImageComparer(IImageDownloader dl, double percentForMatch)
		{
			dl.DownloadsFinished += OnDownloadsFinished;
			PercentForMatch = percentForMatch;
		}

		/// <summary>
		/// Returns false if this was not able to be added to the image comparer's dictionary.
		/// </summary>
		/// <param name="hash">The image's hash.</param>
		/// <param name="bm">The image's bitmap.</param>
		/// <param name="uri">The location the image was downloaded from.</param>
		/// <param name="file">The location where the image will be stored.</param>
		/// <returns>Returns a boolean indicating whether or not the image details were successfully stored.</returns>
		public bool TryStore(string hash, Bitmap bm, Uri uri, FileInfo file)
		{
			if (_Images.TryGetValue(hash, out var alreadyAdded))
			{
				return false;
			}

			return _Images.TryAdd(hash, new ImageDetails(uri, file, CalculateBoolHash(bm), bm.Width, bm.Height));
		}
		/// <summary>
		/// Returns true if successfully able to get a value with <paramref name="hash"/>;
		/// </summary>
		/// <param name="hash">The hash to search for.</param>
		/// <param name="details">The returned value.</param>
		/// <returns>Returns a boolean indicating whether or not the hash is linked to a value.</returns>
		public bool TryGetImage(string hash, out ImageDetails details)
			=> _Images.TryGetValue(hash, out details);
		/// <summary>
		/// Returns true if the amount of matching bools divided by 256 is greater than or equal to <see cref="PercentForMatch"/>.
		/// </summary>
		/// <param name="i1">The first bool hash.</param>
		/// <param name="i2">The second bool hash.</param>
		/// <returns>Returns a boolean indicating whether or not the images are too similar.</returns>
		public static bool CompareImages(ImageDetails i1, ImageDetails i2, double percentForMatch)
			=> (i1.BoolHash.Zip(i2.BoolHash, (i, j) => i == j).Count() / 256.0) >= percentForMatch;

		/// <summary>
		/// When the images have finished downloading run through each of them again to see if any are duplicates.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDownloadsFinished(object sender, EventArgs e)
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
					if (CompareImages(iVal, jVal, PercentForMatch))
					{
						try
						{
							//Delete/remove whatever is the smaller image
							var iTotalPixels = iVal.Width * iVal.Height;
							var jTotalPixels = jVal.Width * jVal.Height;
							//If j is less then delete j
							if (iTotalPixels > jTotalPixels)
							{
								jVal.File.Delete();
								kvps.RemoveAt(j);
							}
							//If less obviously delete, but if equal then delete i since it was downloaded later
							else
							{
								iVal.File.Delete();
								kvps.RemoveAt(i);
							}
							break;
						}
						catch (Exception)
						{
							Console.WriteLine($"Unable to delete the duplicate image {jVal.File}.");
						}
					}
				}
			}
		}

		public static string CalculateMD5Hash(Stream s)
		{
			using (var md5 = MD5.Create())
			{
				return BitConverter.ToString(md5.ComputeHash(s)).Replace("-", "").ToLower();
			}
		}
		public static List<bool> CalculateBoolHash(Bitmap bm)
		{
			var bools = new List<bool>();
			//Create new image with 16x16 pixel
			var smallBm = new Bitmap(bm, new Size(16, 16));
			for (int j = 0; j < smallBm.Height; j++)
			{
				for (int i = 0; i < smallBm.Width; i++)
				{
					//Reduce colors to true / false                
					bools.Add(smallBm.GetPixel(i, j).GetBrightness() < 0.5f);
				}
			}
			return bools;
		}
	}
}
