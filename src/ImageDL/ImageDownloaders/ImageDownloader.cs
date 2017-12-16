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
using System.Threading.Tasks;

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
		/// Downloads all the images that match <paramref name="args"/> then saves all the found animated content links.
		/// </summary>
		/// <param name="args">The arguments to use when searching for images.</param>
		public async Task StartAsync(TArgs args)
		{
			var count = 0;
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
							await DownloadImageAsync(post, args, correctedUri);
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
		protected abstract AnimatedContent StoreAnimatedContentLink(TPost post, Uri uri);
		/// <summary>
		/// Downloads an image from <paramref name="uri"/> and saves it.
		/// </summary>
		/// <param name="post">The post to save from.</param>
		/// <param name="args">The arguments to check against saving.</param>
		/// <param name="uri">The location to the file to save.</param>
		/// <returns></returns>
		protected async Task DownloadImageAsync(TPost post, TArgs args, Uri uri)
		{
			try
			{
				var req = (HttpWebRequest)WebRequest.Create(uri);
				req.Timeout = 5000;
				req.ReadWriteTimeout = 5000;
				using (var resp = await req.GetResponseAsync())
				using (var s = resp.GetResponseStream())
				using (var bm = new Bitmap(s))
				{
					var fileName = (resp.Headers["Content-Disposition"] ?? resp.ResponseUri.LocalPath ?? uri.ToString().Split('/').Last()).Trim('/');
					var savePath = Path.Combine(args.Directory, fileName);
					if (!resp.ContentType.Contains("image"))
					{
						Console.WriteLine($"\t{uri} is not an image.");
						return;
					}
					else if (File.Exists(savePath))
					{
						Console.WriteLine($"\t{fileName} is already saved.");
						return;
					}
					else if (bm == default(Bitmap))
					{
						Console.WriteLine($"\t{uri} is unable to be created as a Bitmap and cannot be saved.");
						return;
					}
					else if (bm.PhysicalDimension.Width < args.MinWidth || bm.PhysicalDimension.Height < args.MinHeight)
					{
						Console.WriteLine($"\t{uri} is too small.");
						return;
					}

					//TODO: async save?
					bm.Save(savePath, ImageFormat.Png);
					Console.WriteLine($"\tSaved {uri} to {savePath}.");
				}
			}
			catch (Exception e)
			{
				e.WriteException();
				using (var writer = new FileInfo(Path.Combine(args.Directory, "FailedDownloads.txt")).AppendText())
				{
					await writer.WriteLineAsync(uri.ToString());
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
