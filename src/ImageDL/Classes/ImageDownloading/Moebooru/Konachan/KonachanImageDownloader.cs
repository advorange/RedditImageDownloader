using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Moebooru.Konachan.Models.KonachanPost;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Konachan
{
	/// <summary>
	/// Downloads images from Konachan.
	/// </summary>
	public sealed class KonachanImageDownloader : MoebooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanImageDownloader"/>.
		/// </summary>
		public KonachanImageDownloader() : base("Konachan", 6) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateKonachanQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseKonachanPosts(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Uri GenerateKonachanQuery(string tags, int page)
		{
			return new Uri($"https://www.konachan.com/post.json" +
				$"?limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&page={page}");
		}
		/// <summary>
		/// Parses Konachan posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static List<Model> ParseKonachanPosts(string text)
		{
			return JsonConvert.DeserializeObject<List<Model>>(text);
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetKonachanPost(IImageDownloaderClient client, string id)
		{
			var result = await client.GetText(GenerateKonachanQuery($"id:{id}", 0)).CAF();
			return result.IsSuccess ? ParseKonachanPosts(result.Value)[0] : null;
		}
	}
}