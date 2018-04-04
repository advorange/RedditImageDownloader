using AdvorangesUtils;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru
{
	/// <summary>
	/// Gathers images from a specified Gelbooru link.
	/// </summary>
	public sealed class GelbooruImageGatherer : MoebooruImageGatherer
	{
		/// <summary>
		/// Creates an instance of <see cref="GelbooruImageGatherer"/>.
		/// </summary>
		public GelbooruImageGatherer() : base("gelbooru.com", "id=", async (c, i) =>
		{
			return await GelbooruImageDownloader.GetGelbooruPostAsync(c, i).CAF();
		}) { }
	}
}
