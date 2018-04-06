using System;
using System.Collections.Generic;

namespace ImageDL.Interfaces
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
		Uri PostUrl { get; }
		/// <summary>
		/// The links to the images in the post.
		/// </summary>
		IEnumerable<Uri> ContentUrls { get; }
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
}