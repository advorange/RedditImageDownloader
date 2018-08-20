using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Booru.Gelbooru.Models.GelbooruPost;

namespace ImageDL.Classes.ImageDownloading.Booru.Gelbooru
{
	/// <summary>
	/// Downloads images from Gelbooru.
	/// </summary>
	[DownloaderName("Gelbooru")]
	public sealed class GelbooruPostDownloader : BooruPostDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="GelbooruPostDownloader"/>.
		/// </summary>
		public GelbooruPostDownloader() : base(int.MaxValue, false) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateGelbooruQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseGelbooruPosts(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateGelbooruQuery(string tags, int page)
		{
			return new Uri($"https://gelbooru.com/index.php" +
				$"?page=dapi" +
				$"&s=post" +
				$"&q=index" +
				$"&json=0" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&pid={Math.Max(0, page - 1)}"); //Pages on Gelbooru are 0 based
		}
		/// <summary>
		/// Parses Gelbooru posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseGelbooruPosts(string text)
		{
			var posts = JObject.Parse(JsonUtils.ConvertXmlToJson(text))["posts"];
			var count = posts["count"].ToObject<int>();
			return count == 1 ? new List<Model> { posts["post"].ToObject<Model>() } : posts["post"].ToObject<List<Model>>();
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetGelbooruPostAsync(IDownloaderClient client, string id)
		{
			var query = GenerateGelbooruQuery($"id:{id}", 0);
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? ParseGelbooruPosts(result.Value)[0] : null;
		}
		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetGelbooruImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var id = HttpUtility.ParseQueryString(url.Query)["id"];
			if (id != null && await GetGelbooruPostAsync(client, id).CAF() is Model post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}