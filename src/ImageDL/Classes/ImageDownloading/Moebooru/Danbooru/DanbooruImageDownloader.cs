using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AdvorangesUtils;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Moebooru.Danbooru.Models.DanbooruPost;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Danbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	public sealed class DanbooruImageDownloader : MoebooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="DanbooruImageDownloader"/>.
		/// </summary>
		public DanbooruImageDownloader() : base("Danbooru", 2) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateDanbooruQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return Parse(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Uri GenerateDanbooruQuery(string tags, int page)
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
		public static List<Model> ParseDanbooruPosts(string text)
		{
			return JsonConvert.DeserializeObject<List<Model>>(text);
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetDanbooruPostAsync(ImageDownloaderClient client, string id)
		{
			var result = await client.GetText(GenerateDanbooruQuery($"id:{id}", 0)).CAF();
			return result.IsSuccess ? ParseDanbooruPosts(result.Value)[0] : null;
		}
	}
}