using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDL.Classes
{
	/// <summary>
	/// Returns the count of things gathered, downloaded, cached, and deleted from an image downloader.
	/// </summary>
	public class DownloaderResponse : Response
	{
		/// <summary>
		/// How many posts were gathered.
		/// </summary>
		public readonly int GatheredPostCount;
		/// <summary>
		/// How many images were downloaded.
		/// </summary>
		public readonly int DownloadedImageCount;
		/// <summary>
		/// How many images were cached.
		/// </summary>
		public readonly int CachedImageCount;
		/// <summary>
		/// How many images were deleted.
		/// </summary>
		public readonly int DeletedImageCount;
		/// <summary>
		/// How many links were added to file.
		/// </summary>
		public readonly int ContentLinksCount;

		/// <summary>
		/// Creates an instance of <see cref="DownloaderResponse"/>.
		/// </summary>
		/// <param name="reasonType"></param>
		/// <param name="text"></param>
		/// <param name="isSuccess"></param>
		/// <param name="gathered"></param>
		/// <param name="downloaded"></param>
		/// <param name="cached"></param>
		/// <param name="deleted"></param>
		/// <param name="links"></param>
		private DownloaderResponse(string reasonType, string text, bool? isSuccess, int gathered, int downloaded, int cached, int deleted, int links)
			: base(reasonType, text, isSuccess)
		{
			GatheredPostCount = gathered;
			DownloadedImageCount = downloaded;
			CachedImageCount = cached;
			DeletedImageCount = deleted;
			ContentLinksCount = links;
		}

		/// <summary>
		/// Returns a downloader response indicating no posts were found.
		/// </summary>
		/// <returns></returns>
		public static DownloaderResponse FromNoPostsFound()
		{
			return new DownloaderResponse("None Found", "Unable to find any new posts.", false, 0, 0, 0, 0, 0);
		}
		/// <summary>
		/// Returns a downloader response indicating the downloader is finished.
		/// </summary>
		/// <param name="gathered"></param>
		/// <param name="downloaded"></param>
		/// <param name="cached"></param>
		/// <param name="deleted"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		public static DownloaderResponse FromFinished(int gathered, int downloaded, int cached, int deleted, int links)
		{
			return new DownloaderResponse("Finished", "Done downloading all images from the gathered posts.", true, gathered, downloaded, cached, deleted, links);
		}
	}
}