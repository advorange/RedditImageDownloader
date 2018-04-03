using AdvorangesUtils;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model = ImageDL.Classes.ImageDownloading.Vsco.VscoPost;

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
		/// <param name="client">The client to download images with.</param>
		public VscoImageDownloader(ImageDownloaderClient client) : base(client, new Uri("http://vsco.co"))
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user" }, x => Username = x)
			{
				Description = "The name of the user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<Model> list)
		{
			var userId = 0;
			var parsed = new List<Model>();
			var keepGoing = true;
			//Iterate because the results are in pages
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count > 0); ++i)
			{
				if (String.IsNullOrWhiteSpace(Client[Name]))
				{
					var (Key, UserId) = await GetKeyAndId().CAF();
					if (Key == null)
					{
						throw new InvalidOperationException("Unable to keep gathering due to being unable to generate a new API token.");
					}
					Client.ApiKeys[Name] = new ApiKey(Key);
					userId = UserId;
				}

				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateQuery(Client[Name], userId, i)).CAF();
				if (!result.IsSuccess)
				{
					//If there's an error with authorization, try to get a new key
					if (result.StatusCode == HttpStatusCode.Unauthorized)
					{
						Client.ApiKeys[Name] = new ApiKey(null);
						--i;
						continue;
					}
					break;
				}

				var page = JsonConvert.DeserializeObject<VscoPage>(result.Text);
				foreach (var post in (parsed = page.Posts))
				{
					if (!(keepGoing = post.UploadedAt >= OldestAllowed))
					{
						break;
					}
					else if (!FitsSizeRequirements(null, post.Width, post.Height, out _))
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

		private async Task<(string Key, int UserId)> GetKeyAndId()
		{
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var keyQuery = new Uri($"http://vsco.co/content/Static/userinfo?callback=jsonp_{time}_0");
			var keyResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(keyQuery).CAF();
			if (!keyResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the api key.");
			}
			var key = Client.Cookies.GetCookies(keyQuery)["vs"].Value;

			var idQuery = new Uri($"http://vsco.co/ajxp/{key}/2.0/sites?subdomain={Username}");
			var idResult = await Client.GetMainTextAndRetryIfRateLimitedAsync(idQuery).CAF();
			if (!idResult.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the user's id.");
			}

			var id = JsonConvert.DeserializeObject<VscoUserResults>(idResult.Text).Users.Single().Id;
			return (key, id);
		}
		private Uri GenerateQuery(string key, int userId, int page)
		{
			return new Uri($"http://vsco.co/ajxp/{key}/2.0/medias" +
				$"?site_id={userId}" +
				$"&page={page}");
		}
	}
}
