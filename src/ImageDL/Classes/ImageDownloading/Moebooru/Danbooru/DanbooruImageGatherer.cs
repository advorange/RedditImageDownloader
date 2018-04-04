using AdvorangesUtils;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Danbooru
{
	/// <summary>
	/// Gathers images from a specified Danbooru link.
	/// </summary>
	public sealed class DanbooruImageGatherer : MoebooruImageGatherer
	{
		/// <summary>
		/// Creates an instance of <see cref="DanbooruImageGatherer"/>.
		/// </summary>
		public DanbooruImageGatherer() : base("donmai.us", "/posts/", async (c, i) =>
		{
			return await DanbooruImageDownloader.GetDanbooruPostAsync(c, i).CAF();
		}) { }
	}
}
