using AdvorangesUtils;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan
{
	/// <summary>
	/// Gathers images from a specified Konachan link.
	/// </summary>
	public sealed class KonachanImageGatherer : MoebooruImageGatherer
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanImageGatherer"/>.
		/// </summary>
		public KonachanImageGatherer() : base("konachan.com", "/post/show/", async (c, i) =>
		{
			return await KonachanImageDownloader.GetKonachanPost(c, i).CAF();
		}) { }
	}
}
