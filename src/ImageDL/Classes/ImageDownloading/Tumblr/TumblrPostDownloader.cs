using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Tumblr.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Tumblr.Models.TumblrPost;

namespace ImageDL.Classes.ImageDownloading.Tumblr
{
	/// <summary>
	/// Downloads images from Tumblr.
	/// </summary>
	[DownloaderName("Tumblr")]
	public sealed class TumblrPostDownloader : PostDownloader
	{
		/// <summary>
		/// The name of the user to download images from.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="TumblrPostDownloader"/>.
		/// </summary>
		public TumblrPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new TumblrPage();
			//Iterate because the results are in pages
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Posts?.Count > 0); i += parsed.Posts?.Count ?? 0)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"http://{Username}.tumblr.com/api/read/json" +
					$"?debug=1" +
					$"&type=photo" +
					$"&filter=text" +
					$"&num=50" +
					$"&start={i}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JsonConvert.DeserializeObject<TumblrPage>(result.Value.Split(new[] { '=' }, 2)[1].Trim().TrimEnd(';'));
				foreach (var post in parsed.Posts)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Score < MinScore)
					{
						continue;
					}
					if (post.Photos.Any()) //Going into here means there is more than one photo
					{
						foreach (var photo in post.Photos.Where(x => !HasValidSize(x, out _)).ToList())
						{
							post.Photos.Remove(photo);
						}
						if (!post.Photos.Any())
						{
							continue;
						}
					}
					else if (!HasValidSize(post, out _)) //Going into here means there is one photo
					{
						continue;
					}
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
		/// <summary>
		/// Gets the link to the full size image.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static Uri GetFullSizeTumblrImage(Uri url)
		{
			//Can't get the raw for inline, and static.tumblr is already full size because they're used for themes.
			if (url.AbsolutePath.CaseInsContains("inline") || url.Host.CaseInsContains("static.tumblr"))
			{
				return url;
			}
			if (url.Host.CaseInsContains("media.tumblr"))
			{
				//Example:
				//https://78.media.tumblr.com/475ede973aab130576a77789c82925b9/tumblr_p5xxjlVAK91td53jko1_1280.jpg
				//https://a.tumblr.com/475ede973aab130576a77789c82925b9/tumblr_p5xxjlVAK91td53jko1_raw.jpg
				var parts = url.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				var file = parts[1];
				var raw = $"{file.Substring(0, file.LastIndexOf('_'))}_raw{Path.GetExtension(file)}";
				return new Uri($"https://a.tumblr.com/{parts[0]}/{raw}");
			}
			return url; //Didn't fit into any of the above, guess just return it?
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetTumblrPostAsync(IDownloaderClient client, string username, string id)
		{
			var query = new Uri($"http://{username}.tumblr.com/api/read/json?debug=1&id={id}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (result.IsSuccess)
			{
				var post = JObject.Parse(result.Value.Split(new[] { '=' }, 2)[1].Trim().TrimEnd(';'))["posts"].First;
				//If the id doesn't match, then that means it just got random values and the id is invalid
				if (post["id"].ToString() == id)
				{
					return post.ToObject<Model>();
				}
			}
			return null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetTumblrImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = GetFullSizeTumblrImage(DownloaderClient.RemoveQuery(url)).ToString().Replace("/post/", "/image/");
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var searches = new[] { "/image/", "/post/" };
			foreach (var search in searches)
			{
				if (u.CaseInsIndexOf(search, out var index))
				{
					var username = url.Host.Split('.')[0];
					var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
					if (await GetTumblrPostAsync(client, username, id).CAF() is Model post)
					{
						return await post.GetImagesAsync(client).CAF();
					}
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}
