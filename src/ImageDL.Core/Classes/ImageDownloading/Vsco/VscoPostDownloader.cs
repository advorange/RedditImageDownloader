using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesSettingParser.Implementation.Instance;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Vsco.Models;
using ImageDL.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Model = ImageDL.Classes.ImageDownloading.Vsco.Models.VscoPost;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Downloads images from Vsco.
	/// </summary>
	[DownloaderName("Vsco")]
	public sealed class VscoPostDownloader : PostDownloader
	{
		private static readonly Type _Type = typeof(VscoPostDownloader);

		/// <summary>
		/// The name of the user to download images from.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="VscoPostDownloader"/>.
		/// </summary>
		public VscoPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Username, new[] { "user" })
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <summary>
		/// Gets an api key for Vsco.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<ApiKey> GetApiKeyAsync(IDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(_Type, out var key))
			{
				return key;
			}
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var query = new Uri($"https://vsco.co/content/Static/userinfo?callback=jsonp_{time}_0");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the api key.");
			}
			var cookie = client.Cookies.GetCookies(query)["vs"].Value;
			return client.ApiKeys[_Type] = new ApiKey(cookie);
		}

		/// <summary>
		/// Gets the id of the Vsco user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<ulong> GetUserIdAsync(IDownloaderClient client, string username)
		{
			var query = new Uri($"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/sites?subdomain={username}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the user's id.");
			}
			return JsonConvert.DeserializeObject<VscoUserResults>(result.Value).Users.Single().Id;
		}

		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetVscoImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			const string search = "/media/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetVscoPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetVscoPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/medias/{id}");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
			return result.IsSuccess ? JObject.Parse(result.Value)["media"].ToObject<Model>() : null;
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = 0UL;
			var parsed = new VscoPage();
			//Iterate because the results are in pages
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Posts.Count > 0); ++i)
			{
				token.ThrowIfCancellationRequested();
				if (userId == 0UL)
				{
					userId = await GetUserIdAsync(client, Username).CAF();
				}

				var query = new Uri($"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/medias" +
					$"?site_id={userId}" +
					$"&page={i}");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with authorization, try to get a new key
					if (result.StatusCode == HttpStatusCode.Unauthorized)
					{
						client.ApiKeys.Remove(_Type);
						--i;
						continue;
					}
					return;
				}

				parsed = JsonConvert.DeserializeObject<VscoPage>(result.Value);
				foreach (var post in parsed.Posts)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post, out _))
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
	}
}