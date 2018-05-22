using System;
using System.Threading.Tasks;
using ImageDL.Classes;

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
		/// The score of the post. (Not necessarily always score, e.g. Tumblr returns note count)
		/// </summary>
		int Score { get; }
		/// <summary>
		/// The time the post was created at.
		/// </summary>
		DateTime CreatedAt { get; }

		/// <summary>
		/// Returns all the images from the post.
		/// </summary>
		/// <returns></returns>
		Task<ImageResponse> GetImagesAsync(IDownloaderClient client);
	}
}