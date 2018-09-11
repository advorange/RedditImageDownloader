using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Pinterest.Models.PinterestPost;

namespace ImageDL.Classes.ImageDownloading.Pinterest
{
	/// <summary>
	/// Downloads images from Pinterest.
	/// </summary>
	[DownloaderName("Pinterest")]
	public sealed class PinterestPostDownloader : PostDownloader
	{
		/// <summary>
		/// The value to search for. Can be a user, board, or tags.
		/// </summary>
		public string Search { get; set; }
		/// <summary>
		/// How to use the search string.
		/// </summary>
		public PinterestGatheringMethod GatheringMethod { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="PinterestPostDownloader"/>.
		/// </summary>
		public PinterestPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Search) }, x => Search = x)
			{
				Description = $"The text to search for. Can be a board, user, or tags. The respective type must be set in {GatheringMethod}.",
			});
			SettingParser.Add(new Setting<PinterestGatheringMethod>(new[] { nameof(GatheringMethod), "method" }, x => GatheringMethod = x,
				parser: s => (Enum.TryParse(s, true, out PinterestGatheringMethod result), result))
			{
				Description = $"How to use {Search} to search through Pinterest.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var id = "";
			//Iterate to deal with pagination
			for (string bm = ""; list.Count < AmountOfPostsToGather && bm != "-end-";) //-end- is used to indicate the pagination is done
			{
				token.ThrowIfCancellationRequested();
				//Every search method has these tags, so they're outside the switch
				var options = new Dictionary<string, object>
				{
					{ "page_size", 250 }, //Max allowed amount per iteration
					{ "bookmarks", new string[] { bm } }, //Pagination
					{ "field_set_key", "detailed" }, //Get the most information
				};
				var endpoint = "";
				switch (GatheringMethod)
				{
					case PinterestGatheringMethod.Board:
						if (id == "")
						{
							id = await GetBoardId(client, new Uri($"https://www.pinterest.com/{Search.TrimStart('/')}"));
						}
						options.Add("board_id", id); //The id of the board to search
						endpoint = $"/BoardFeedResource/get/";
						break;
					case PinterestGatheringMethod.User:
						options.Add("username", Search); //User to search for
						endpoint = $"/UserPinsResource/get/";
						break;
					case PinterestGatheringMethod.Tags:
						options.Add("query", Search); //Tags to search for
						options.Add("scope", "pins"); //Specify to search through pins
						endpoint = $"/BaseSearchResource/get/";
						break;
					default:
						throw new InvalidOperationException("Invalid search type provided.");
				}

				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(GenerateQuery(endpoint, options));
					req.Headers.Add("X-Requested-With", "XMLHttpRequest");
					return req;
				});
				if (!result.IsSuccess)
				{
					return;
				}

				var jObj = JObject.Parse(result.Value);
				bm = jObj["resource"]["options"]["bookmarks"][0].ToObject<string>();
				List<Model> posts;
				switch (GatheringMethod)
				{
					case PinterestGatheringMethod.Board:
					case PinterestGatheringMethod.User:
						posts = jObj["resource_response"]["data"].ToObject<List<Model>>();
						break;
					case PinterestGatheringMethod.Tags:
						posts = jObj["resource_response"]["data"]["results"].ToObject<List<Model>>();
						break;
					default:
						throw new InvalidOperationException("Invalid search type provided.");
				}

				foreach (var post in posts)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post.LargestImage, out _) || post.Score < MinScore)
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
		/// Gets the id of a board.
		/// </summary>
		/// <returns></returns>
		private static async Task<string> GetBoardId(IDownloaderClient client, Uri url)
		{
			var result = await client.GetTextAsync(() => client.GenerateReq(url)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException("Unable to get the board id.");
			}

			var search = "\"board_id\": \"";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return cut.Substring(0, cut.IndexOf('"'));
		}
		/// <summary>
		/// Generates a query to access the Pinterest API.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private static Uri GenerateQuery(string endpoint, Dictionary<string, object> options)
		{
			return new Uri($"https://www.pinterest.com/resource{endpoint}" +
				$"?data={JsonConvert.SerializeObject(new Dictionary<string, object> { { "options", options }, })}" +
				$"&_={(long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds}");
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetPinterestPostAsync(IDownloaderClient client, string id)
		{
			var options = new Dictionary<string, object>
			{
				{ "id", id },
				{ "field_set_key", "detailed" },
			};
			var endpoint = "/PinResource/get/";
			var result = await client.GetTextAsync(() =>
			{
				var req = client.GenerateReq(GenerateQuery(endpoint, options));
				req.Headers.Add("X-Requested-With", "XMLHttpRequest");
				return req;
			});
			return result.IsSuccess ? JObject.Parse(result.Value)["resource_response"]["data"].ToObject<Model>() : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetPinterestImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/pin/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetPinterestPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}