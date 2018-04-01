using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ImageDL.Classes.ImageDownloading.Booru.Danbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	public sealed class DanbooruImageDownloader : BooruImageDownloader<DanbooruPost>
	{
		/// <summary>
		/// Creates an instance of <see cref="DanbooruImageDownloader"/>.
		/// </summary>
		public DanbooruImageDownloader() : base("Danbooru", 2) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(int page)
		{
			return new Uri($"https://danbooru.donmai.us/posts.json" +
				$"?utf8=✓" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&page={page}");
		}
		/// <inheritdoc />
		protected override List<DanbooruPost> Parse(string text)
		{
			return JsonConvert.DeserializeObject<List<DanbooruPost>>(text);
		}
	}
}