using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Vsco.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Vsco.Models.VscoPost;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Downloads images from Vsco.
	/// </summary>
	public sealed class VscoImageDownloader : ImageDownloader<Model>
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
		/// Creates an instance of <see cref="VscoImageDownloader"/>.
		/// </summary>
		public VscoImageDownloader() : base("Vsco")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			var userId = 0;
			var parsed = new List<Model>();
			var keepGoing = true;
			//Iterate because the results are in pages
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count > 0); ++i)
			{
				if (userId == 0)
				{
					userId = await GetUserIdAsync(client, Username).CAF();
				}

				var query = $"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/medias" +
					$"?site_id={userId}" +
					$"&page={i}";
				var result = await client.GetText(new Uri(query)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with authorization, try to get a new key
					if (result.StatusCode == HttpStatusCode.Unauthorized)
					{
						client.ApiKeys.Remove(typeof(VscoImageDownloader));
						--i;
						continue;
					}
					break;
				}

				var page = JsonConvert.DeserializeObject<VscoPage>(result.Value);
				foreach (var post in (parsed = page.Posts))
				{
					if (!(keepGoing = post.UploadedAt >= OldestAllowed))
					{
						break;
					}
					else if (!HasValidSize(null, post.Width, post.Height, out _))
					{
						continue;
					}
					if (!(keepGoing = Add(list, post)))
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets an api key for Vsco.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<ApiKey> GetApiKeyAsync(IImageDownloaderClient client)
		{
			if (client.ApiKeys.TryGetValue(typeof(VscoImageDownloader), out var key))
			{
				return key;
			}
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var query = $"https://vsco.co/content/Static/userinfo?callback=jsonp_{time}_0";
			var result = await client.GetText(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the api key.");
			}
			var cookie = client.Cookies.GetCookies(new Uri(query))["vs"].Value;
			return (client.ApiKeys[typeof(VscoImageDownloader)] = new ApiKey(cookie));
		}
		/// <summary>
		/// Gets the id of the Vsco user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		public static async Task<int> GetUserIdAsync(IImageDownloaderClient client, string username)
		{
			var query = $"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/sites?subdomain={username}";
			var result = await client.GetText(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the user's id.");
			}
			return JsonConvert.DeserializeObject<VscoUserResults>(result.Value).Users.Single().Id;
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetVscoPostAsync(IImageDownloaderClient client, string id)
		{
			var query = $"https://vsco.co/ajxp/{await GetApiKeyAsync(client).CAF()}/2.0/medias/{id}";
			var result = await client.GetText(new Uri(query)).CAF();
			return result.IsSuccess ? JObject.Parse(result.Value)["media"].ToObject<Model>() : null;
		}
	}
}
