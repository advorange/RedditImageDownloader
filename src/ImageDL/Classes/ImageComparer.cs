using ImageDL.ImageDownloaders;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
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
		/// <param name="file">The file location that is being added.</param>
		/// <param name="s">The content of the file.</param>
		/// <param name="alreadyAdded">Whether or not something has already been added.</param>
		/// <returns></returns>
		public bool TryAddValue(FileInfo file, Stream s, out ImageDetails alreadyAdded)
		{

		}
		public bool TryGetImage(string hash, out FileInfo info)
		{
			var response = _Images.TryGetValue(hash, out var value);
			info = response ? value.File : default;
			return response;
		}

		private void OnDownloadsFinished(object sender, EventArgs e)
		{

		}

		private string CalculateMD5(Stream s)
		{
			using (var md5 = MD5.Create())
			{
				return BitConverter.ToString(md5.ComputeHash(s)).Replace("-", "").ToLower();
			}
		}
	}
}
