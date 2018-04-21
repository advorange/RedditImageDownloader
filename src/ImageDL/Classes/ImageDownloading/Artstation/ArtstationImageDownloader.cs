using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Artstation.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Artstation.Models.ArtstationPost;

namespace ImageDL.Classes.ImageDownloading.Artstation
{
	/// <summary>
	/// Downloads images from Artstation.
	/// </summary>
	public sealed class ArtstationImageDownloader : ImageDownloader
	{
		/// <summary>
		/// The name of the user to search for.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="ArtstationImageDownloader"/>.
		/// </summary>
		public ArtstationImageDownloader() : base("Artstation")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user", }, x => Username = x)
			{
				Description = "The user to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var parsed = new ArtstationPage();
			//Iterate to get the next page of results
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Posts.Count >= 50); ++i)
			{
				var query = new Uri($"https://www.artstation.com/users/{Username}/projects.json?page={i}");
				var result = await client.GetText(() => client.GetReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = JsonConvert.DeserializeObject<ArtstationPage>(result.Value);
				foreach (var post in parsed.Posts)
				{
					var fullPost = await GetArtstationPostAsync(client, post.Id);
					if (fullPost.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (fullPost.Score < MinScore)
					{
						continue;
					}
					//Remove all images that don't meet the size requirements
					foreach (var image in fullPost.Assets.Where(x => x.AssetType != "image" || !HasValidSize(x, out _)).ToList())
					{
						fullPost.Assets.Remove(image);
					}
					if (!fullPost.Assets.Any())
					{
						continue;
					}
					if (!Add(list, fullPost))
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
		public static async Task<Model> GetArtstationPostAsync(IImageDownloaderClient client, string id)
		{
			var query = new Uri($"https://www.artstation.com/projects/{id}.json");
			var result = await client.GetText(() => client.GetReq(query)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<Model>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetArtstationImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/artwork/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/')[0];
				if (await GetArtstationPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}