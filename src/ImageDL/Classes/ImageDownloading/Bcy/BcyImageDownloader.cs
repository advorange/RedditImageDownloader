using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Model = ImageDL.Interfaces.IPost;

namespace ImageDL.Classes.ImageDownloading.Bcy
{
	/// <summary>
	/// Downloads images from Bcy.
	/// </summary>
	[DownloaderName("Bcy")]
	public sealed class BcyImageDownloader : ImageDownloader
	{
		/// <summary>
		/// The id of the user to search for.
		/// </summary>
		public ulong UserId
		{
			get => _UserId;
			set => _UserId = value;
		}

		private ulong _UserId;

		/// <summary>
		/// Creates an instance of <see cref="BcyImageDownloader"/>
		/// </summary>
		public BcyImageDownloader()
		{
			SettingParser.Add(new Setting<ulong>(new[] { nameof(UserId), "id" }, x => UserId = x)
			{
				Description = "The id of the user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || true); ++i)
			{
				var data = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					{ "uid", UserId.ToString() },
					{ "since", (i * 20).ToString() },
					{ "limit", 20.ToString() },
					{ "filter", "origin" },
					{ "source", "all" },
				});
				var query = new Uri("https://bcy.net/home/user/loadtimeline");
				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(query, HttpMethod.Post);
					req.Content = data;
					req.Headers.Add("X-Requested-With", "XMLHttpRequest");
					return req;
				}).CAF();
				if (!result.IsSuccess)
				{
					return;
				}


			}
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetBcyPostAsync(IImageDownloaderClient client, string id)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetBcyImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/illust/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').Last();
				if (await GetBcyPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}