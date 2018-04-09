using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvorangesUtils;
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
	public sealed class TumblrImageDownloader : ImageDownloader<Model>
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
		/// Creates an instance of <see cref="TumblrImageDownloader"/>.
		/// </summary>
		public TumblrImageDownloader() : base("Tumblr")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			IList<Model> parsed = new List<Model>();
			//Iterate because the results are in pages
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count > 0); i += parsed.Count)
			{
				var query = new Uri($"http://{Username}.tumblr.com/api/read/json" +
					$"?debug=1" +
					$"&type=photo" +
					$"&filter=text" +
					$"&num=50" +
					$"&start={i}");
				var result = await client.GetText(client.GetReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var page = JsonConvert.DeserializeObject<TumblrPage>(result.Value);
				foreach (var post in (parsed = page.Posts))
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					else if (!HasValidSize(null, post.Width, post.Height, out _))
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
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetTumblrPostAsync(IImageDownloaderClient client, string username, string id)
		{
			var query = new Uri($"http://{username}.tumblr.com/api/read/json?debug=1&id={id}");
			var result = await client.GetText(client.GetReq(query)).CAF();
			if (result.IsSuccess)
			{
				var post = JObject.Parse(result.Value)["posts"].First;
				//If the id doesn't match, then that means it just got random values and the id is invalid
				if (post["id"].ToString() == id)
				{
					return post.ToObject<Model>();
				}
			}
			return null;
		}
	}
}
