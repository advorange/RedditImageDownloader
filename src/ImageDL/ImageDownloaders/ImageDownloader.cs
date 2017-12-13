using ImageDL.Classes;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ImageDownloader<T> where T : ImageDownloaderArguments
	{
		protected List<AnimatedContent> _AnimatedContent = new List<AnimatedContent>();

		/// <summary>
		/// Downloads all the images that match <paramref name="args"/> then saves all the found animated content.
		/// </summary>
		/// <param name="args">The arguments to use when searching for images.</param>
		public void Start(T args)
		{
			DownloadImages(args);
			SaveFoundAnimatedContentUris(new DirectoryInfo(args.Folder));
		}
		protected abstract void DownloadImages(T args);
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it.
		/// </summary>
		/// <param name="uri">The image location.</param>
		/// <param name="directory">The directory to save to.</param>
		/// <param name="fileName">The name to give it.</param>
		protected void DownloadImage(Uri uri, DirectoryInfo directory, string fileName)
		{
			//Don't bother redownloading files
			var savePath = Path.Combine(directory.FullName, fileName);
			if (File.Exists(savePath))
			{
				Console.WriteLine($"{fileName} is already saved.");
				return;
			}

			try
			{
				var req = (HttpWebRequest)WebRequest.Create(uri);
				req.Timeout = 5000;
				req.ReadWriteTimeout = 5000;
				using (var resp = req.GetResponse())
				using (var s = resp.GetResponseStream())
				{
					if (!resp.ContentType.Contains("image"))
					{
						Console.WriteLine($"\t{uri} is not an image.");
						return;
					}

					var bitmap = new Bitmap(s);
					if (bitmap == null)
					{
						Console.WriteLine($"\t{uri} is unable to be created as a Bitmap and cannot be saved.");
						return;
					}
					//TODO: supply these as arguments
					else if (bitmap.PhysicalDimension.Width < 200 || bitmap.PhysicalDimension.Height < 200)
					{
						Console.WriteLine($"\t{uri} is too small.");
						return;
					}

					bitmap.Save(savePath, ImageFormat.Png);
					Console.WriteLine($"\tSuccessfully downloaded {uri}.");
				}
			}
			catch (Exception e)
			{
				e.WriteException();
				using (var writer = new FileInfo(Path.Combine(directory.FullName, "FailedDownloads.txt")).AppendText())
				{
					writer.WriteLine(uri);
				}
			}
		}
		/// <summary>
		/// Saves all the links gotten to animated content.
		/// </summary>
		/// <param name="directory">The folder to save to.</param>
		protected void SaveFoundAnimatedContentUris(DirectoryInfo directory)
		{
			if (!_AnimatedContent.Any())
			{
				return;
			}
			var fileInfo = new FileInfo(Path.Combine(directory.FullName, "Animated_Content.txt"));

			//Only save links which are not already in the text document
			var unsavedAnimatedContent = new List<AnimatedContent>();
			using (var reader = new StreamReader(fileInfo.OpenRead()))
			{
				var text = reader.ReadToEnd();
				foreach (var anim in _AnimatedContent)
				{
					if (!text.Contains(anim.Uri.ToString()))
					{
						unsavedAnimatedContent.Add(anim);
					}
				}
			}

			if (!unsavedAnimatedContent.Any())
			{
				return;
			}

			//Save all the links then say how many were saved
			var scoreLength = unsavedAnimatedContent.Max(x => x.Score).GetLengthOfNumber();
			using (var writer = fileInfo.AppendText())
			{
				writer.WriteLine($"Animated Content - {Utils.FormatDateTimeForSaving()}");
				foreach (var anim in unsavedAnimatedContent)
				{
					writer.WriteLine($"{anim.Score.ToString().PadLeft(scoreLength, '0')} {anim.Uri}");
				}
				writer.WriteLine();
			}
			Console.WriteLine($"Added {unsavedAnimatedContent.Count()} links to {fileInfo.Name}.");
		}
	}
}
