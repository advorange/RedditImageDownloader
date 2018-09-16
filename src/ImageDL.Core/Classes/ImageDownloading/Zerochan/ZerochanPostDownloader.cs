using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser.Implementation.Instance;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Zerochan.Models.ZerochanPost;

namespace ImageDL.Classes.ImageDownloading.Zerochan
{
	/// <summary>
	/// Downloads images from Zerochan.
	/// </summary>
	[DownloaderName("Zerochan")]
	public sealed class ZerochanPostDownloader : PostDownloader
	{
		/// <summary>
		/// The terms to search for.
		/// </summary>
		public string Tags { get; set; }

		/// <summary>
		/// Creats an instance of <see cref="ZerochanPostDownloader"/>.
		/// </summary>
		public ZerochanPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Tags)
			{
				Description = "The tags to search for.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<Model>();
			for (int i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 24); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri($"https://www.zerochan.net/{WebUtility.UrlEncode(Tags)}" +
					$"?s=id" +
					$"&p={i + 1}" +
					$"&json");
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var items = JObject.Parse(result.Value)["items"];
				parsed = (await Task.WhenAll(items.Select(async x => await GenerateModel(client, x).CAF()))).ToList();
				foreach (var post in parsed)
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
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
		/// <summary>
		/// Generates a model with gotten json and the id/tags from <paramref name="jToken"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="jToken"></param>
		/// <returns></returns>
		private static async Task<Model> GenerateModel(IDownloaderClient client, JToken jToken)
		{
			var query = new Uri($"https://www.zerochan.net/{jToken["id"]}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}

			var script = result.Value.DocumentNode.Descendants("script");
			var json = script.Single(x => x.GetAttributeValue("type", null) == "application/ld+json");
			var jObj = JObject.Parse(json.InnerText);

			//Add in the tags and id
			foreach (var innerToken in jToken.OfType<JProperty>())
			{
				if (!jObj.ContainsKey(innerToken.Name))
				{
					jObj.Add(innerToken);
				}
			}

			return jObj.ToObject<Model>();
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetZerochanPostAsync(IDownloaderClient client, string id) => await GenerateModel(client, new JObject { { "id", id } }).CAF();
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetZerochanImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var parts = url.LocalPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1 && int.TryParse(parts[0], out var val) && await GetZerochanPostAsync(client, val.ToString()).CAF() is Model post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}