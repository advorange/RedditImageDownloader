using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Lofter.Models.LofterPost;

namespace ImageDL.Classes.ImageDownloading.Lofter
{
	/// <summary>
	/// Downloads images from Lofter.
	/// </summary>
	[DownloaderName("Lofter")]
	public sealed class LofterPostDownloader : PostDownloader
	{
		/// <summary>
		/// The username to search for.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="LofterPostDownloader"/>.
		/// </summary>
		public LofterPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Username, new[] { "user" })
			{
				Description = "The user to download images from.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = await GetUserIdAsync(client, Username).CAF();
			var parsed = new List<Model>();
			for (long ts = 0; list.Count < AmountOfPostsToGather && (ts == 0 || parsed.Count >= 50); ts = parsed.Last().CreatedAtTimestamp)
			{
				token.ThrowIfCancellationRequested();
				ts = ts == 0 ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds() : ts;

				var query = new Uri($"http://{Username}.lofter.com/dwr/call/plaincall/ArchiveBean.getArchivePostByTime.dwr");
				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(query, HttpMethod.Post);
					req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
					{
						{ "callCount", "1" },
						{ "scriptSessionId", "${scriptSessionId}187" },
						{ "httpSessionId", "" },
						{ "c0-scriptName", "ArchiveBean" },
						{ "c0-methodName", "getArchivePostByTime" },
						{ "c0-id", "0" },
						{ "c0-param0", $"number:{userId}" }, //User id
						{ "c0-param1", $"number:{ts}" }, //Timestamp
						{ "c0-param2", $"number:50" }, //Posts per req
						{ "c0-param3", $"boolean:false" },
						{ "batchId", "1" },
					});
					return req;
				}).CAF();

				parsed = JsonConvert.DeserializeObject<List<Model>>(ConvertJsToJson(result.Value));
				foreach (var post in parsed)
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
					await post.FillPost(client).CAF();
					foreach (var image in post.Images.Where(x => !HasValidSize(x, out _)).ToList())
					{
						post.Images.Remove(image);
					}
					if (!post.Images.Any())
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
		/// Gets the id of the Lofter user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<string> GetUserIdAsync(IDownloaderClient client, string username)
		{
			var query = new Uri($"http://{username}.lofter.com");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();

			var search = "blogId=";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return cut.Substring(0, cut.IndexOf('"'));
		}
		/// <summary>
		/// Converts the supplied Javascript object creation to JSON.
		/// </summary>
		/// <param name="js"></param>
		/// <returns></returns>
		private static string ConvertJsToJson(string js)
		{
			//The request gives back Javascript object creation instead of JSON (no clue why, so we need to do this)
			var split = js.ComplexSplit(new[] { ';' }, new[] { '"' }, true).Select(x => x.Trim());
			var jArray = new JArray();
			for (int i = 0; i < 50; ++i)
			{
				var jObjInner = new JObject();
				foreach (var part in split.Where(x => x.StartsWith($"s{i}.") || x.StartsWith($"s{i + 50}.")))
				{
					var kvp = part.Substring(part.IndexOf('.') + 1).Split(new[] { '=' }, 2);
					//First value is the name, second value is the actual value
					var name = kvp[0].FormatTitle().Replace(' ', '_').ToLower(); //Put into standard JSON title format
					var value = kvp[1];
					if (value.StartsWith("\""))
					{
						value = value.Substring(1);
					}
					if (value.EndsWith("\""))
					{
						value = value.Substring(0, value.Length - 1);
					}
					jObjInner.Add(name, value);
				}
				jArray.Add(jObjInner);
			}
			return jArray.ToString();
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetLofterImagesAsync(IDownloaderClient client, Uri url)
		{
			var result = await client.GetHtmlAsync(() => client.GenerateReq(url)).CAF();
			var div = result.Value.DocumentNode.Descendants("div");
			var pics = div.Where(x => x.GetAttributeValue("class", null) == "pic").Select(x => x.Descendants("a").Single());
			var urls = pics.Select(x => new Uri(x.GetAttributeValue("bigimgsrc", null).Split('?')[0]));
			return ImageResponse.FromImages(urls);
		}
	}
}