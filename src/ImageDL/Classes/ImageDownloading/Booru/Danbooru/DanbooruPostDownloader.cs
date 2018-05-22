using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Booru.Danbooru.Models.DanbooruPost;

namespace ImageDL.Classes.ImageDownloading.Booru.Danbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	[DownloaderName("Danbooru")]
	public sealed class DanbooruPostDownloader : BooruPostDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="DanbooruPostDownloader"/>.
		/// </summary>
		public DanbooruPostDownloader() : base(2) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateDanbooruQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseDanbooruPosts(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateDanbooruQuery(string tags, int page)
		{
			return new Uri($"https://danbooru.donmai.us/posts.json" +
				$"?utf8=✓" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&page={page}");
		}
		/// <summary>
		/// Parses Danbooru posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseDanbooruPosts(string text)
		{
			return JsonConvert.DeserializeObject<List<Model>>(text);
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetDanbooruPostAsync(IDownloaderClient client, string id)
		{
			var query = GenerateDanbooruQuery($"id:{id}", 0);
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? ParseDanbooruPosts(result.Value)[0] : null;
		}
		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetDanbooruImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/posts/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetDanbooruPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}