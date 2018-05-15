using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Classes.ImageDownloading.Pawoo.Models.PawooPost;

namespace ImageDL.Classes.ImageDownloading.Pawoo
{
	/// <summary>
	/// Downloads images from Pawoo.
	/// </summary>
	[DownloaderName("Pawoo")]
	public sealed class PawooImageDownloader : ImageDownloader
	{
		private static readonly Type _Type = typeof(PawooImageDownloader);

		/// <summary>
		/// The user to search for images from.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}
		/// <summary>
		/// Whether or not to include replies when searching through a user's posts.
		/// </summary>
		public bool IncludeReplies
		{
			get => _IncludeReplies;
			set => _IncludeReplies = value;
		}

		private string _Username;
		private bool _IncludeReplies;

		/// <summary>
		/// Creates an instance of <see cref="PawooImageDownloader"/>.
		/// </summary>
		public PawooImageDownloader()
		{
			//Make sure the username always has an @ in front of it
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user", }, x => Username = x, s => (true, s.StartsWith("@") ? s : $"@{s}"))
			{
				Description = "The user to search for. Will end up with an @ in front of it if one is not supplied.",
			});
			SettingParser.Add(new Setting<bool>(new[] { nameof(IncludeReplies), "replies", }, x => IncludeReplies = x)
			{
				Description = "Whether to search through replies in addition to regular posts.",
				IsFlag = true,
			});
		}

		/// <inheritdoc />
		public override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			var id = 0UL;
			var parsed = new List<Model>();
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 20); ++i)
			{
				//If the id is 0 this just started so it should be gotten
				if (id == 0UL)
				{
					id = await GetUserIdAsync(client, Username).CAF();
				}

				var query = new Uri($"https://pawoo.net/api/v1/accounts/{id}/statuses" +
					$"?access_token={await GetApiKeyAsync(client).CAF()}" +
					$"&max_id={(parsed.Any() ? parsed.Last().Id : "")}" +
					$"&exclude_replies={(IncludeReplies ? "0" : "1")}" +
					$"&only_media=1");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with the access token, try to get another one
					if (result.Value.Contains("access token"))
					{
						//Means the access token cannot be gotten
						if (!client.ApiKeys.ContainsKey(_Type))
						{
							return;
						}
						client.ApiKeys.Remove(_Type);
						--i; //Decrement since this iteration is useless
						continue;
					}
					return;
				}

				parsed = JsonConvert.DeserializeObject<List<Model>>(result.Value);
				foreach (var post in parsed)
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (post.Score < MinScore)
					{
						continue;
					}
					foreach (var image in post.MediaAttachments.Where(x => !HasValidSize(x.Meta.Original, out _)).ToList())
					{
						post.MediaAttachments.Remove(image);
					}
					if (!post.MediaAttachments.Any())
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
		/// Logs into Pawoo, generating an access token.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}
			//Load a random user's page so we can scrape the API key from it
			var query = new Uri($"https://pawoo.net/web/accounts/{new Random().Next(100000)}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the access token.");
			}
			var search = "\"access_token\":\"";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return (client.ApiKeys[_Type] = new ApiKey(cut.Substring(0, cut.IndexOf('"'))));
		}
		/// <summary>
		/// Returns the id of the user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<ulong> GetUserIdAsync(IImageDownloaderClient client, string username)
		{
			var query = new Uri($"https://pawoo.net/{username}/");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the user id.");
			}
			var search = "/api/salmon/";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return Convert.ToUInt64(cut.Substring(0, cut.IndexOf('\'')));
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetPawooPostAsync(IImageDownloaderClient client, string id)
		{
			//This API call does not require authentication.
			var query = new Uri($"https://www.pawoo.net/api/v1/statuses/{id}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<Model>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetPawooImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/statuses/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetPawooPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			if (u.Contains("@"))
			{
				var id = u.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
				if (await GetPawooPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}