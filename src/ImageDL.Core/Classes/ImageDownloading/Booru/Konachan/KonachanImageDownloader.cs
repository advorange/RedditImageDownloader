using System;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Downloads images from Konachan.
	/// </summary>
	public sealed class KonachanImageDownloader : BooruImageDownloader<KonachanPost>
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanImageDownloader"/>.
		/// </summary>
		public KonachanImageDownloader() : base(new Uri("https://www.danbooru.donmai.us"), "post.json", 6) { }
	}
}