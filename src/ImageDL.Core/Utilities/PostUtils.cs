using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using AdvorangesUtils;
using ImageDL.Classes;
using ImageDL.Core.Enums;
using ImageDL.Interfaces;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Utilities for <see cref="IPost"/>.
	/// </summary>
	public static class PostUtils
	{
		/// <summary>
		/// Generates the file to use for this uri.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="dir"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static FileInfo GenerateFileInfo(this IPost post, DirectoryInfo dir, Uri url)
		{
			var path = Path.GetInvalidFileNameChars()
				.Aggregate(Path.GetFileName(url.ToString()), (p, c) => p.Replace(c.ToString(), ""));
			var directory = dir.ToString();
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(path)}";
			var extension = Path.GetExtension(path);

			//Make sure the extension has a period
			extension = extension.StartsWith(".") ? extension : "." + extension;
			//Remove any invalid file name path characters
			name = new string(name.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
			//Max file name length has to be under 260 for windows, but 256 for some other things, so just go with 255.
			var nameLen = 255 - directory.Length - 1 - extension.Length; //Subtract extra 1 for / between dir and file
			return new FileInfo(Path.Combine(directory, name.Substring(0, Math.Min(name.Length, nameLen)) + extension));
		}
		/// <summary>
		/// Creates a content link of the object.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="url"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public static ContentLink CreateContentLink(this IPost post, Uri url, Response response)
			=> new ContentLink(url, post.Score, response.ReasonType);
		/// <summary>
		/// Returns the count, the score, and the ToString() of the post.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string Format(this IPost post, int count)
			=> $"[#{count}{(post.Score >= 0 ? $"|\u2191{post.Score}" : "")}] {post.PostUrl}";
		/// <summary>
		/// Gets the size of an image from a post. If the post implements the <see cref="ISize"/> interface, then returns that instead of reading the stream.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="ms"></param>
		/// <returns></returns>
		public static (int Width, int Height) GetSize(this IPost post, MemoryStream ms)
		{
			if (post is ISize size)
			{
				return (size.Width, size.Height);
			}
			else
			{
				ms.Seek(0, SeekOrigin.Begin);
				var both = ms.GetImageSize();
				ms.Seek(0, SeekOrigin.Begin);
				return both;
			}
		}
		/// <summary>
		/// Determines if the data is a valid image to download.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="resp"></param>
		/// <returns></returns>
		public static ImageResponseStatus GetImageResponseType(this HttpResponseMessage resp)
		{
			if (!resp.IsSuccessStatusCode)
			{
				return ImageResponseStatus.Fail;
			}
			var ct = resp.Content.Headers.GetValues("Content-Type").First();
			if (ct.Contains("video/") || ct == "image/gif")
			{
				return ImageResponseStatus.Animated;
			}
			//If the content type is image, then we know it's an image
			if (ct.Contains("image/"))
			{
				return ImageResponseStatus.Image;
			}
			//If the content type is octet-stream then we need to check the url path and assume its extension is correct
			var url = resp.RequestMessage.RequestUri;
			if (ct == "application/octet-stream" && url.ToString().IsImagePath())
			{
				return ImageResponseStatus.Image;
			}
			//If the content type is force download then we need to check the content disposition
			if (ct == "application/force-download"
				&& resp.Content.Headers.TryGetValues("Content-Disposition", out var cd)
				&& new ContentDisposition(cd.Single()).FileName.IsImagePath())
			{
				return ImageResponseStatus.Image;
			}
			return ImageResponseStatus.NotImage;
		}
		/// <summary>
		/// Writes to the console if the number is not zero.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="str"></param>
		public static void WriteIfNotZero(int number, Func<string> str)
		{
			if (number != 0)
			{
				ConsoleUtils.WriteLine(str.Invoke());
			}
		}
	}
}