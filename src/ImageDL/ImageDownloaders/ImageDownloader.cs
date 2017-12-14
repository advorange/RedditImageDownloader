using ImageDL.Classes;
using ImageDL.Enums;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace ImageDL.ImageDownloaders
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	/// <typeparam name="TArgs"></typeparam>
	public abstract class ImageDownloader<TArgs, TPost> where TArgs : ImageDownloaderArguments
	{
		private List<AnimatedContent> _AnimatedContent = new List<AnimatedContent>();

		/// <summary>
		/// Downloads all the images that match <paramref name="args"/> then saves all the found animated content.
		/// </summary>
		/// <param name="args">The arguments to use when searching for images.</param>
		public void Start(TArgs args)
		{
			var count = 1;
			foreach (var post in GatherPosts(args))
			{
				Thread.Sleep(25);
				WritePostToConsole(post, ++count);
				foreach (var imageUri in GatherImages(post))
				{
					switch (UriUtils.CorrectUri(imageUri, out var correctedUri))
					{
						case UriCorrectionResponse.Valid:
						case UriCorrectionResponse.Unknown:
						{
							DownloadImage(correctedUri, new DirectoryInfo(args.Directory), GenerateFileName(post, correctedUri));
							continue;
						}
						case UriCorrectionResponse.Animated:
						{
							_AnimatedContent.Add(StoreAnimatedContentLink(post, correctedUri));
							continue;
						}
						case UriCorrectionResponse.Invalid:
						{
							continue;
						}
					}
				}
			}
			SaveFoundAnimatedContentUris(new DirectoryInfo(args.Directory));
		}
		protected abstract IEnumerable<TPost> GatherPosts(TArgs args);
		protected abstract IEnumerable<Uri> GatherImages(TPost post);
		protected abstract void WritePostToConsole(TPost post, int count);
		protected abstract string GenerateFileName(TPost post, Uri uri);
		protected abstract AnimatedContent StoreAnimatedContentLink(TPost post, Uri uri);
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
				Console.WriteLine($"\t{fileName} is already saved.");
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
					Console.WriteLine($"\tSaved {uri} to {savePath}.");
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
			if (fileInfo.Exists)
			{
				using (var reader = new StreamReader(fileInfo.OpenRead()))
				{
					var text = reader.ReadToEnd();
					foreach (var anim in _AnimatedContent)
					{
						if (text.Contains(anim.Uri.ToString()))
						{
							continue;
						}

						unsavedAnimatedContent.Add(anim);
					}
				}
			}
			else
			{
				unsavedAnimatedContent = _AnimatedContent;
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
