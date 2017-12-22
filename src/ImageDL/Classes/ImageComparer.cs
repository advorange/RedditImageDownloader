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

		public ImageComparer(IImageDownloader dl)
		{
			dl.DownloadsFinished += OnDownloadsFinished;
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

			var boolHash = CalculateBoolHash(bm);
			return _Images.TryAdd(hash, new ImageDetails(uri, file, boolHash));
		}
		public bool TryGetImage(string hash, out ImageDetails details)
			=> _Images.TryGetValue(hash, out details);

		private bool CompareImages(ImageDetails i1, ImageDetails i2)
		{
			return true;
			//return i1.BoolHash.Zip(i2.BoolHash, (i, j) => i == j).Count();
		}

		private void OnDownloadsFinished(object sender, EventArgs e)
		{

		}

		public static string CalculateMD5(Stream s)
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
