using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using Model = ImageDL.Classes.ImageDownloading.Booru.Konachan.KonachanPost;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Downloads images from Konachan.
	/// </summary>
	public sealed class KonachanImageDownloader : BooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanImageDownloader"/>.
		/// </summary>
		/// <param name="client">The client to download images with.</param>
		public KonachanImageDownloader(ImageDownloaderClient client) : base(client, new Uri("http://konachan.com"), 6) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(int page)
		{
			return new Uri($"https://www.konachan.com/post.json" +
				$"?limit=100" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&page={page}");
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return JsonConvert.DeserializeObject<List<Model>>(text);
		}
	}
}