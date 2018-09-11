using System;
using System.IO;
using System.Linq;
using ImageDL.Classes;
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
		public static ContentLink CreateContentLink(this IPost post, Uri url, Response response) => new ContentLink(url, post.Score, response.ReasonType);
		/// <summary>
		/// Returns the count, the score, and the ToString() of the post.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string Format(this IPost post, int count) => $"[#{count}{(post.Score >= 0 ? $"|\u2191{post.Score}" : "")}] {post.PostUrl}";
	}
}