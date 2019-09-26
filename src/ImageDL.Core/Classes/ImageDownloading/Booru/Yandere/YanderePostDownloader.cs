using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Interfaces;

using Newtonsoft.Json;

using Model = ImageDL.Classes.ImageDownloading.Booru.Yandere.Models.YanderePost;

namespace ImageDL.Classes.ImageDownloading.Booru.Yandere
{
	/// <summary>
	/// Downloads images from Yandere.
	/// </summary>
	[DownloaderName("Yandere")]
	public sealed class YanderePostDownloader : BooruPostDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="YanderePostDownloader"/>.
		/// </summary>
		public YanderePostDownloader() : base(6) { }

		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetYandereImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			const string search = "/post/show/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetYanderePostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetYanderePostAsync(IDownloaderClient client, string id)
		{
			var query = GenerateYandereQuery($"id:{id}", 0);
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? ParseYanderePosts(result.Value)[0] : null;
		}

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page) => GenerateYandereQuery(tags, page);

		/// <inheritdoc />
		protected override List<Model> Parse(string text) => ParseYanderePosts(text);

		/// <summary>
		/// Generates a search url.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateYandereQuery(string tags, int page)
		{
			return new Uri("https://www.yande.re/post.json" +
				"?limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&page={page}");
		}

		/// <summary>
		/// Parses Yandere posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseYanderePosts(string text) => JsonConvert.DeserializeObject<List<Model>>(text);
	}
}