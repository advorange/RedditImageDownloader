using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Booru.Konachan.Models.KonachanPost;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Downloads images from Konachan.
	/// </summary>
	[DownloaderName("Konachan")]
	public sealed class KonachanPostDownloader : BooruPostDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanPostDownloader"/>.
		/// </summary>
		public KonachanPostDownloader() : base(6) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page) => GenerateKonachanQuery(tags, page);
		/// <inheritdoc />
		protected override List<Model> Parse(string text) => ParseKonachanPosts(text);

		/// <summary>
		/// Generates a search url.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateKonachanQuery(string tags, int page)
		{
			//Not sure why, but requires Http over Https otherwise returns gibberish
			return new Uri($"http://www.konachan.com/post.json" +
				$"?utf8=✓" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&page={page}");
		}
		/// <summary>
		/// Parses Konachan posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseKonachanPosts(string text) => JsonConvert.DeserializeObject<List<Model>>(text);
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetKonachanPostAsync(IDownloaderClient client, string id)
		{
			var query = GenerateKonachanQuery($"id:{id}", 0);
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? ParseKonachanPosts(result.Value)[0] : null;
		}
		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetKonachanImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/post/show/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetKonachanPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}