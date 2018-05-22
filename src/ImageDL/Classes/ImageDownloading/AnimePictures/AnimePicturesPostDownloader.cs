using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.AnimePictures.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.AnimePictures.Models.AnimePicturesPost;

namespace ImageDL.Classes.ImageDownloading.AnimePictures
{
	/// <summary>
	/// Downloads images from AnimePictures.
	/// </summary>
	[DownloaderName("Anime-Pictures")]
	public sealed class AnimePicturesPostDownloader : PostDownloader
	{
		/// <summary>
		/// The terms to search with.
		/// </summary>
		public string Tags
		{
			get => _Tags;
			set => _Tags = value;
		}

		private string _Tags;

		/// <summary>
		/// Creates an instance of <see cref="AnimePicturesPostDownloader"/>.
		/// </summary>
		public AnimePicturesPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new AnimePicturesPage();
			//Iterate because there's a limit of around 100 per request
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.ResponsePostsCount >= 60); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://anime-pictures.net/pictures/view_posts/{i}" +
					$"?search_tag={WebUtility.UrlEncode(Tags)}" +
					$"&type=json" +
					$"&order_by=date" +
					$"&ldate=0" +
					$"&lang=en");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JsonConvert.DeserializeObject<AnimePicturesPage>(result.Value);
				foreach (var post in parsed.Posts)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post, out _) || post.Score < MinScore)
					{
						continue;
					}
					if (!Add(list, await GetAnimePicturesPostAsync(client, post.Id)))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetAnimePicturesPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"https://anime-pictures.net/pictures/view_post/{id}?type=json&lang=en");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<Model>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetAnimePicturesImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/view_post/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetAnimePicturesPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}