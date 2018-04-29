using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Model = ImageDL.Interfaces.IPost;
using ImageDL.Enums;
using System.Net;
using ImageDL.Classes.ImageDownloading.Pinterest.Models;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Pinterest
{
	/// <summary>
	/// Downloads images from Pinterest.
	/// </summary>
	[DownloaderName("Pinterest")]
	public sealed class PinterestImageDownloader : ImageDownloader
	{
		/// <summary>
		/// The value to search for. Can be a user, board, or tags.
		/// </summary>
		public string Search
		{
			get => _Search;
			set => _Search = value;
		}
		/// <summary>
		/// How to use the search string.
		/// </summary>
		public PinterestSearchType GatheringMethod
		{
			get => _GatheringMethod;
			set => _GatheringMethod = value;
		}

		private string _Search;
		private PinterestSearchType _GatheringMethod;

		/// <summary>
		/// Creates an instance of <see cref="PinterestImageDownloader"/>.
		/// </summary>
		public PinterestImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Search) }, x => Search = x)
			{
				Description = $"The text to search for. Can be a board, user, or tags. The respective type must be set in {GatheringMethod}.",
			});
			SettingParser.Add(new Setting<PinterestSearchType>(new[] { nameof(GatheringMethod), "method" }, x => GatheringMethod = x, s => (Enum.TryParse(s, true, out PinterestSearchType result), result))
			{
				Description = $"How to use {Search} to search through Pinterest.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			var id = "";
			//Iterate to deal with pagination
			for (string bm = ""; list.Count < AmountOfPostsToGather && bm != "-end";) //-end- is used to indicate the pagination is done
			{
				//Every search method has these tags, so they're outside the switch
				var options = new Dictionary<string, object>
				{
					{ "page_size", 250 }, //Max allowed amount per iteration
					{ "bookmarks", new string[] { bm } }, //Pagination
				};
				var query = "https://www.pinterest.com/resource";
				switch (GatheringMethod)
				{
					case PinterestSearchType.Board:
						if (id == "")
						{
							id = await GetBoardId(client, new Uri($"https://www.pinterest.com/{Search.TrimStart('/')}"));
						}
						options.Add("board_id", id); //The id of the board to search
						query += $"/BoardFeedResource/get/?source_url={WebUtility.UrlEncode($"/{Search.TrimStart('/')}")}";
						break;
					case PinterestSearchType.User:
						options.Add("username", Search); //User to search for
						query += $"/UserPinsResource/get/?source_url=/{Search}/pins/";
						break;
					case PinterestSearchType.Tags:
						options.Add("query", Search); //Tags to search for
						options.Add("scope", "pins"); //Specify to search through pins
						query = $"/BaseSearchResource/get/?source_url=/search/pins/?q={Search}&rs=typed";
						break;
					default:
						throw new InvalidOperationException("Invalid search type provided.");
				}

				query += $"&data={JsonConvert.SerializeObject(new Dictionary<string, object>{ { "options", options }, })}";
				query += $"&_={(long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds}";
				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(new Uri(query));
					req.Headers.Add("X-Requested-With", "XMLHttpRequest");
					return req;
				});
				if (!result.IsSuccess)
				{
					return;
				}

				var jObj = JObject.Parse(result.Value);
				bm = jObj["resource"]["options"]["bookmarks"][0].ToObject<string>();
				List<PinterestPost> posts;
				switch (GatheringMethod)
				{
					case PinterestSearchType.Board:
					case PinterestSearchType.User:
						posts = jObj["resource_response"]["data"].ToObject<List<PinterestPost>>();
						break;
					case PinterestSearchType.Tags:
						posts = jObj["resource_response"]["data"]["results"].ToObject<List<PinterestPost>>();
						break;
					default:
						throw new InvalidOperationException("Invalid search type provided.");
				}

				foreach (var post in posts)
				{
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post.LargestImage, out _))
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
		private static async Task<string> GetBoardId(IImageDownloaderClient client, Uri url)
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
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetPinterestImagesAsync(IImageDownloaderClient client, Uri url)
		{
			throw new NotImplementedException();
		}
	}
}