using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Moebooru.Yandere.Models.YanderePost;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Yandere
{
	/// <summary>
	/// Downloads images from Yandere.
	/// </summary>
	public sealed class YandereImageDownloader : MoebooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="YandereImageDownloader"/>.
		/// </summary>
		public YandereImageDownloader() : base("Yandere", 6) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateYandereQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseYanderePosts(text);
		}

		/// <summary>
		/// Generates a search url.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateYandereQuery(string tags, int page)
		{
			return new Uri($"https://www.yande.re/post.json" +
				$"?limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&page={page}");
		}
		/// <summary>
		/// Parses Yandere posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseYanderePosts(string text)
		{
			return JsonConvert.DeserializeObject<List<Model>>(text);
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetYanderePostAsync(IImageDownloaderClient client, string id)
		{
			var query = GenerateYandereQuery($"id:{id}", 0);
			var result = await client.GetText(() => client.GetReq(query)).CAF();
			return result.IsSuccess ? ParseYanderePosts(result.Value)[0] : null;
		}
		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetYandereImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/post/show/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/')[0];
				if (await GetYanderePostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}