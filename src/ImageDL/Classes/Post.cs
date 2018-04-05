using ImageDL.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageDL.Classes
{
	/// <summary>
	/// Interface for a post.
	/// </summary>
	public interface IPost
	{
		/// <summary>
		/// The id of the post.
		/// </summary>
		string Id { get; }
		/// <summary>
		/// The direct link to the post.
		/// </summary>
		string PostUrl { get; }
		/// <summary>
		/// The links to the images in the post.
		/// </summary>
		IEnumerable<string> ContentUrls { get; }
		/// <summary>
		/// The score of the post. (Not necessarily always score, e.g. Tumblr returns note count)
		/// </summary>
		int Score { get; }
		/// <summary>
		/// The time the post was created at.
		/// </summary>
		DateTime CreatedAt { get; }

		/// <summary>
		/// Returns a string representing the post.
		/// </summary>
		/// <returns></returns>
		string ToString();
	}

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
		/// <param name="uri"></param>
		/// <returns></returns>
		public static FileInfo GenerateFileInfo(IPost post, DirectoryInfo dir, Uri uri)
		{
			var directory = dir.ToString();
			var name = $"{post.Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			var extension = Path.GetExtension(uri.LocalPath);

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
		/// <param name="uri"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		public static ContentLink CreateContentLink(IPost post, Uri uri, FailureReason reason)
		{
			return new ContentLink(uri, post.Score, reason);
		}
		/// <summary>
		/// Returns the count, the score, and the ToString() of the post.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string Format(IPost post, int count)
		{
			return $"[#{count}|\u2191{(post.Score >= 0 ? $"|\u2191{post.Score}" : "")}] {post.ToString()}";
		}
	}
}
